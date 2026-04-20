using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Application.Features.Interviews.Mappers;
using CareerLens.Application.Features.Interviews.Queries.Employee.GetInterviewById;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Interviews;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Queries.Employer.GetInterviewById
{
    [RequireRole("Employer")]
    public sealed record GetInterviewByIdForEmployerQuery(Guid CompanyId, Guid InterviewId)
        : IRequest<Result<IInterviewResponse>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles =>
        [
            CompanyMemberRole.Moderator
        ];
    }

    public sealed class GetInterviewByIdForEmployerQueryValidator : AbstractValidator<GetInterviewByIdForEmployerQuery>
    {
        public GetInterviewByIdForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.InterviewId)
                .NotEmpty()
                .WithMessage("Interview ID is required.");
        }
    }
    public sealed class GetInterviewByIdForEmployerQueryHandler(IAppDbContext context)
        : IRequestHandler<GetInterviewByIdForEmployerQuery, Result<IInterviewResponse>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<IInterviewResponse>> Handle(GetInterviewByIdForEmployerQuery request, CancellationToken cancellationToken)
        {

            var interview = await _context.Interviews
            .Include(i => i.User)
            .Include(i => i.InterviewQuestions)
            .FirstOrDefaultAsync(i => i.Id == request.InterviewId
                                   && i.CompanyId == request.CompanyId,
                                 cancellationToken);

            if (interview is null)
            {
                return ApplicationErrors.InterviewNotFound;
            }

            return interview.ToEmployerDto();
        }
    }
}
