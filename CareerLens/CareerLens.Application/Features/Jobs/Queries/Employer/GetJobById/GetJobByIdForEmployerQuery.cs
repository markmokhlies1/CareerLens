using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Application.Features.Jobs.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Jobs.Queries.Employer.GetJobById
{
    [RequireRole("Employer")]
    public sealed record GetJobByIdForEmployerQuery(Guid JobId,Guid CompanyId) : IRequest<Result<IJobResponse>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Hr];
    }
    public sealed class GetJobByIdForEmployerQueryValidator : AbstractValidator<GetJobByIdForEmployerQuery>
    {
        public GetJobByIdForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID is required.");

            RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("Job ID is required.");
        }
    }
    public sealed class GetJobByIdForEmployerQueryHandler(IAppDbContext context)
        : IRequestHandler<GetJobByIdForEmployerQuery, Result<IJobResponse>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<IJobResponse>> Handle(GetJobByIdForEmployerQuery request, CancellationToken cancellationToken)
        {
            var job = await _context.Jobs
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

            if (job is null)
            {
                return ApplicationErrors.JobNotFound;
            }

            return job.ToEmployerDto();
        }
    }
}
