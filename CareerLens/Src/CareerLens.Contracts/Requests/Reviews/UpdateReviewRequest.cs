using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Reviews.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Reviews
{
    public class UpdateReviewRequest
    {
        public int OverallRating { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public JobFunction JobFunction { get; set; }
        public LengthOfEmployment LengthOfEmployment { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string Pros { get; set; } = string.Empty;
        public string Cons { get; set; } = string.Empty;

        public string? JobTitle { get; set; }
        public string? AdviceToManagement { get; set; }
        public string? Location { get; set; }

        public int? CareerOpportunities { get; set; }
        public int? CompensationAndBenefits { get; set; }
        public int? CultureAndValues { get; set; }
        public int? DiversityAndInclusion { get; set; }
        public int? SeniorManagement { get; set; }
        public int? WorkLifeBalance { get; set; }

        public Sentiment? CeoRating { get; set; }
        public Sentiment? RecommendToFriend { get; set; }
        public Sentiment? BusinessOutlook { get; set; }
    }

    public sealed class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.OverallRating)
                .InclusiveBetween(1, 5)
                .WithMessage("Overall rating must be between 1 and 5.");

            RuleFor(x => x.EmploymentStatus)
                .IsInEnum()
                .WithMessage("Invalid employment status.");

            RuleFor(x => x.JobFunction)
                .IsInEnum()
                .WithMessage("Invalid job function.");

            RuleFor(x => x.LengthOfEmployment)
                .IsInEnum()
                .WithMessage("Invalid length of employment.");

            RuleFor(x => x.EmployeeType)
                .IsInEnum()
                .WithMessage("Invalid employee type.");

            RuleFor(x => x.Headline)
                .NotEmpty()
                .WithMessage("Headline is required.")
                .MaximumLength(200)
                .WithMessage("Headline must not exceed 200 characters.");

            RuleFor(x => x.Pros)
                .NotEmpty()
                .WithMessage("Pros is required.")
                .Must(p => Helper.CountWords(p) >= CareerLensConstants.InterviewReviewPronsLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Pros))
                .WithMessage($"Pros must be at least {CareerLensConstants.InterviewReviewPronsLength} words.");

            RuleFor(x => x.Cons)
                .NotEmpty()
                .WithMessage("Cons is required.")
                .Must(c => Helper.CountWords(c) >= CareerLensConstants.InterviewReviewConsLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Cons))
                .WithMessage($"Cons must be at least {CareerLensConstants.InterviewReviewConsLength} words.");

            RuleFor(x => x.JobTitle)
                .MaximumLength(150)
                .WithMessage("Job title must not exceed 150 characters.")
                .When(x => x.JobTitle is not null);

            RuleFor(x => x.AdviceToManagement)
                .MaximumLength(1000)
                .WithMessage("Advice to management must not exceed 1000 characters.")
                .When(x => x.AdviceToManagement is not null);

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage("Location must not exceed 200 characters.")
                .When(x => x.Location is not null);

            RuleFor(x => x.CareerOpportunities)
                .InclusiveBetween(1, 5)
                .WithMessage("Career opportunities must be between 1 and 5.")
                .When(x => x.CareerOpportunities.HasValue);

            RuleFor(x => x.CompensationAndBenefits)
                .InclusiveBetween(1, 5)
                .WithMessage("Compensation and benefits must be between 1 and 5.")
                .When(x => x.CompensationAndBenefits.HasValue);

            RuleFor(x => x.CultureAndValues)
                .InclusiveBetween(1, 5)
                .WithMessage("Culture and values must be between 1 and 5.")
                .When(x => x.CultureAndValues.HasValue);

            RuleFor(x => x.DiversityAndInclusion)
                .InclusiveBetween(1, 5)
                .WithMessage("Diversity and inclusion must be between 1 and 5.")
                .When(x => x.DiversityAndInclusion.HasValue);

            RuleFor(x => x.SeniorManagement)
                .InclusiveBetween(1, 5)
                .WithMessage("Senior management must be between 1 and 5.")
                .When(x => x.SeniorManagement.HasValue);

            RuleFor(x => x.WorkLifeBalance)
                .InclusiveBetween(1, 5)
                .WithMessage("Work life balance must be between 1 and 5.")
                .When(x => x.WorkLifeBalance.HasValue);

            RuleFor(x => x.CeoRating)
                .IsInEnum()
                .WithMessage("Invalid CEO rating.")
                .When(x => x.CeoRating.HasValue);

            RuleFor(x => x.RecommendToFriend)
                .IsInEnum()
                .WithMessage("Invalid recommend to friend value.")
                .When(x => x.RecommendToFriend.HasValue);

            RuleFor(x => x.BusinessOutlook)
                .IsInEnum()
                .WithMessage("Invalid business outlook value.")
                .When(x => x.BusinessOutlook.HasValue);
        }
    }
}
