using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Salaries;
using CareerLens.Domain.Salaries.Enums;
using CareerLens.Domain.Salaries.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.Commands.Employer.UpdateSalaryState
{
    [RequireRole("Employer")]
    public sealed record UpdateSalaryStatusCommand(Guid SalaryId, Guid CompanyId, SalaryStatus Status)
    : IRequest<Result<Updated>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class UpdateSalaryStatusCommandValidator
    : AbstractValidator<UpdateSalaryStatusCommand>
    {
        private static readonly SalaryStatus[] AllowedStatuses =
        [
            SalaryStatus.Approved,
            SalaryStatus.Rejected
        ];

        public UpdateSalaryStatusCommandValidator()
        {
            RuleFor(x => x.SalaryId)
                .NotEmpty()
                .WithMessage("Salary ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid salary status.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage("Status must be Approved or Rejected.");
        }
    }
    public sealed class UpdateSalaryStatusCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateSalaryStatusCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<Updated>> Handle(UpdateSalaryStatusCommand request,CancellationToken cancellationToken)
        {
            
            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.Id == request.SalaryId, cancellationToken);

            if (salary is null)
            {
                return ApplicationErrors.SalaryNotFound;
            }

            var updateResult = salary.UpdateStatus(request.Status);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            if (request.Status == SalaryStatus.Approved)
            {
                salary.AddDomainEvent(new SalaryApproved(salary.Id, salary.UserId, salary.JobTitle!));
            }

            if (request.Status == SalaryStatus.Rejected)
            {
                salary.AddDomainEvent(new SalaryRejected(salary.Id, salary.UserId, salary.JobTitle!));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
