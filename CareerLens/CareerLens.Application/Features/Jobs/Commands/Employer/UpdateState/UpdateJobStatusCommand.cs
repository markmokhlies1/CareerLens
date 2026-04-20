using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Jobs;
using CareerLens.Domain.Jobs.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Jobs.Commands.Employer.UpdateState
{
    [RequireRole("Employer")]
    public sealed record UpdateJobStatusCommand(Guid JobId, Guid CompanyId, JobStatus Status)
    : IRequest<Result<Updated>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Hr];
    }

    public sealed class UpdateJobStatusCommandValidator
    : AbstractValidator<UpdateJobStatusCommand>
    {
        public UpdateJobStatusCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required."); 

            RuleFor(x => x.JobId)
                .NotEmpty()
                .WithMessage("Job ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid job status.");
        }
    }
    public sealed class UpdateJobStatusCommandHandler(IAppDbContext context)
        : IRequestHandler<UpdateJobStatusCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Updated>> Handle(
            UpdateJobStatusCommand request,
            CancellationToken cancellationToken)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

            if (job is null)
            {
                return ApplicationErrors.JobNotFound;
            }

            var updateResult = job.UpdateStatus(request.Status);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
