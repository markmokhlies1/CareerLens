using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Application.Features.Interviews.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Queries.Employee.GetInterviewById
{
    [RequireRole("Employee")]
    public sealed record GetInterviewByIdForEmployeeQuery(Guid InterviewId) 
        : IRequest<Result<IInterviewResponse>>;

    public sealed class GetInterviewByIdQueryValidator : AbstractValidator<GetInterviewByIdForEmployeeQuery>
    {
        public GetInterviewByIdQueryValidator()
        {

            RuleFor(x => x.InterviewId)
                .NotEmpty()
                .WithMessage("Interview ID is required.");
        }
    }

    public sealed class GetInterviewByIdForEmployeeQueryHandler (IAppDbContext context)
        : IRequestHandler<GetInterviewByIdForEmployeeQuery, Result<IInterviewResponse>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<IInterviewResponse>> Handle(GetInterviewByIdForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var interview = await _context.Interviews
            .AsNoTracking()
            .Include(i => i.User)
            .Include(i => i.InterviewQuestions)
            .FirstOrDefaultAsync(i => i.Id == request.InterviewId, cancellationToken);

            if (interview is null)
            {
                return ApplicationErrors.InterviewNotFound;
            }

            return interview.ToBasicDto();
        }
    }
}
