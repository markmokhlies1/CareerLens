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

namespace CareerLens.Application.Features.Jobs.Commands.Employer.UpdateJob
{
    [RequireRole("Employer")]
    public sealed record UpdateJobCommand(Guid JobId,
                                          Guid CompanyId,
                                          string Title,
                                          string Description,
                                          string Location,
                                          EmploymentType EmploymentType,
                                          WorkplaceType WorkplaceType,
                                          ExperienceLevel ExperienceLevel,
                                          decimal MinSalary,
                                          decimal MaxSalary,
                                          PayPeriod PayPeriod,
                                          string ApplyUrl) : IRequest<Result<Updated>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Hr];
    }

    public sealed class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
    {
        public UpdateJobCommandValidator() 
        {
            RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("Job ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Location is required.");

            RuleFor(x => x.EmploymentType)
                .IsInEnum()
                .WithMessage("Invalid employment type.");

            RuleFor(x => x.WorkplaceType)
                .IsInEnum()
                .WithMessage("Invalid workplace type.");

            RuleFor(x => x.ExperienceLevel)
                .IsInEnum()
                .WithMessage("Invalid experience level.");

            RuleFor(x => x.PayPeriod)
                .IsInEnum()
                .WithMessage("Invalid pay period.");

            RuleFor(x => x.MinSalary)
                .GreaterThan(0)
                .WithMessage("Minimum salary must be greater than 0.");

            RuleFor(x => x.MaxSalary)
                .GreaterThan(0)
                .WithMessage("Maximum salary must be greater than 0.")
                .GreaterThanOrEqualTo(x => x.MinSalary)
                .WithMessage("Maximum salary must be greater than or equal to minimum salary.");

            RuleFor(x => x.ApplyUrl)
                .NotEmpty()
                .WithMessage("Apply URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.ApplyUrl))
                .WithMessage("Apply URL must be a valid URL.");
        }
    }

    public sealed class UpdateJobCommandHandler(IAppDbContext context) : IRequestHandler<UpdateJobCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Updated>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == request.JobId
                                   && j.CompanyId == request.CompanyId,
                                 cancellationToken);

            if (job is null)
            {
                return ApplicationErrors.JobNotFound;
            }

            var updateResult = job.Update(title: request.Title,
                                          description: request.Description,
                                          location: request.Location,
                                          employmentType: request.EmploymentType,
                                          workplaceType: request.WorkplaceType,
                                          experienceLevel: request.ExperienceLevel,
                                          minSalary: request.MinSalary,
                                          maxSalary: request.MaxSalary,
                                          payPeriod: request.PayPeriod,
                                          applyUrl: request.ApplyUrl);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
