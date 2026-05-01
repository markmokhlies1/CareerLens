using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Interviews.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Interviews
{
    public class CreateInterviewRequest
    {
        public Guid CompanyId {  get; set; }
        public OverallExperience OverallExperience { get; set; }
        public InterviewDifficulty InterviewDifficulty { get; set; }
        public GettingOffer GettingOffer { get; set; }
        public string? JobTitle {  get; set; }
        public string? Description { get; set; }
        public InterviewSource? Source { get; set; }
        public HelpingLevel? HelpingLevel { get; set; }
        public string? Location { get; set; }
        public int? DurationValue { get; set; }
        public InterviewDurationUnit? DurationUnit {  get; set; }
        public int? DateYear {  get; set; }
        public int? DateMonth { get; set; }
        public InterviewStage? Stages {  get; set; }
        public List<CreateInterviewQuestionRequest> Questions { get; set; } = default!;
    }
    public sealed class CreateInterviewRequestValidator
    : AbstractValidator<CreateInterviewRequest>
    {
        public CreateInterviewRequestValidator()
        {
            
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company is required.");

            RuleFor(x => x.JobTitle)
                .NotEmpty()
                .WithMessage("Job title is required.")
                .MaximumLength(150)
                .WithMessage("Job title must not exceed 150 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .Must(d => Helper.CountWords(d) >= CareerLensConstants.InterviewDescriptionLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage($"Description must be at least {CareerLensConstants.InterviewDescriptionLength} words.");


            RuleFor(x => x.OverallExperience)
                .IsInEnum()
                .WithMessage("Invalid overall experience value.");

            RuleFor(x => x.InterviewDifficulty)
                .IsInEnum()
                .WithMessage("Invalid interview difficulty value.");

            RuleFor(x => x.GettingOffer)
                .IsInEnum()
                .WithMessage("Invalid getting offer value.");

            RuleFor(x => x.Source)
                .IsInEnum()
                .When(x => x.Source.HasValue)
                .WithMessage("Invalid source value.");

            RuleFor(x => x.HelpingLevel)
                .IsInEnum()
                .When(x => x.HelpingLevel.HasValue)
                .WithMessage("Invalid helping level value.");

            RuleFor(x => x.Stages)
                .IsInEnum()
                .When(x => x.Stages.HasValue)
                .WithMessage("Invalid interview stage value.");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage("Location must not exceed 200 characters.")
                .When(x => x.Location is not null);

            RuleFor(x => x.DurationUnit)
                .NotNull()
                .WithMessage("Duration unit is required when duration value is provided.")
                .IsInEnum()
                .WithMessage("Invalid duration unit value.")
                .When(x => x.DurationValue.HasValue);

            RuleFor(x => x.DurationValue)
                .NotNull()
                .WithMessage("Duration value is required when duration unit is provided.")
                .When(x => x.DurationUnit.HasValue);

            RuleFor(x => x.DurationValue)
                .GreaterThan(0)
                .WithMessage("Duration value must be greater than zero.")
                .When(x => x.DurationValue.HasValue);

            RuleFor(x => x.DateMonth)
                .NotNull()
                .WithMessage("Date month is required when date year is provided.")
                .When(x => x.DateYear.HasValue);

            RuleFor(x => x.DateYear)
                .NotNull()
                .WithMessage("Date year is required when date month is provided.")
                .When(x => x.DateMonth.HasValue);

            RuleFor(x => x.DateYear)
                .GreaterThanOrEqualTo(2000)
                .WithMessage("Date year must be 2000 or later.")
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage($"Date year must not exceed the current year ({DateTime.UtcNow.Year}).")
                .When(x => x.DateYear.HasValue);

            RuleFor(x => x.DateMonth)
                .InclusiveBetween(1, 12)
                .WithMessage("Date month must be between 1 and 12.")
                .When(x => x.DateMonth.HasValue);

            RuleFor(x => x.Questions)
                .NotNull()
                .WithMessage("Questions list is required.")
                .NotEmpty()
                .WithMessage("At least one question is required.");

            RuleForEach(x => x.Questions)
                .SetValidator(new CreateInterviewQuestionRequestValidator())
                .When(x => x.Questions is not null);
        }
    }
}
