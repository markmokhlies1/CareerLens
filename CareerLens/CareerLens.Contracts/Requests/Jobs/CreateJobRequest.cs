using CareerLens.Domain.Jobs.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Jobs
{
    public class CreateJobRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public EmploymentType EmploymentType { get; set; }
        public WorkplaceType WorkplaceType { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public PayPeriod PayPeriod { get; set; }
        public string ApplyUrl { get; set; } = string.Empty;
    }

    public sealed class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
    {
        public CreateJobRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Location is required.")
                .MaximumLength(200)
                .WithMessage("Location must not exceed 200 characters.");

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
}
