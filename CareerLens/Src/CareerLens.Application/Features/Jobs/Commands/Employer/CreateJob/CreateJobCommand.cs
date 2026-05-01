
using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Application.Features.Jobs.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Jobs;
using CareerLens.Domain.Jobs.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerLens.Application.Features.Jobs.Commands.Employer.CreateJob
{
    [RequireRole("Employer")]
    public sealed record CreateJobCommand(Guid CompanyId,
                                           string Title,
                                           string Description,
                                           string Location,
                                           EmploymentType EmploymentType,
                                           WorkplaceType WorkplaceType,
                                           ExperienceLevel ExperienceLevel,
                                           decimal MinSalary,
                                           decimal MaxSalary,
                                           PayPeriod PayPeriod,
                                           string ApplyUrl)
        : IRequest<Result<IJobResponse>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Hr];
    }
    public sealed class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        public CreateJobCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

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
    public sealed class CreateJobCommandHandler(IAppDbContext context, IUser currentUser)
        : IRequestHandler<CreateJobCommand, Result<IJobResponse>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<IJobResponse>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var jobResult = Job.Create(
                id: Guid.NewGuid(),
                companyId: request.CompanyId,
                postedByUserId: userId,
                title: request.Title,
                description: request.Description,
                location: request.Location,
                employmentType: request.EmploymentType,
                workplaceType: request.WorkplaceType,
                experienceLevel: request.ExperienceLevel,
                minSalary: request.MinSalary,
                maxSalary: request.MaxSalary,
                payPeriod: request.PayPeriod,
                applyUrl: request.ApplyUrl
            );

            if (jobResult.IsError)
            {
                return jobResult.Errors;
            }

            var job = jobResult.Value;

            await _context.Jobs.AddAsync(job, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return job.ToEmployerDto();
        }
    }
}
