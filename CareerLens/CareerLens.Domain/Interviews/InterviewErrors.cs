using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Interviews
{
    public static class InterviewErrors
    {
        public static Error IdRequired =>
            Error.Validation("Interview.Id.Required", "Interview Id is required.");

        public static Error UserIdRequired =>
            Error.Validation("Interview.UserId.Required", "User Id is required.");

        public static Error CompanyIdRequired =>
            Error.Validation("Interview.CompanyId.Required", "Company Id is required.");

        public static Error JobTitleRequired =>
            Error.Validation("Interview.JobTitle.Required", "Job title is required.");

        public static Error DescriptionRequired =>
            Error.Validation("Interview.Description.Required", "Interview description is required.");

        public static Error CannotApproveNonPendingInterview =>
            Error.Conflict(
                "Interview.CannotApproveNonPendingInterview",
                "Only Interviews with Pending status can be approved.");

        public static Error CannotRejectNonPendingInterview =>
            Error.Conflict(
                "Interview.CannotRejectNonPendingInterview",
                "Only Interviews with Pending status can be Rejected.");

        public static Error DescriptionTooShort =>
            Error.Validation(
                "Interview.Description.TooShort",
                "Interview description must contain at least 30 words.");

        public static Error QuestionsRequired =>
            Error.Validation(
                "Interview.Questions.Required",
                "At least one interview question must be provided.");

        public static Error OverallExperienceInvalid =>
            Error.Validation(
                "Interview.OverallExperience.Invalid",
                "Invalid overall interview experience.");

        public static Error InterviewDifficultyInvalid =>
            Error.Validation(
                "Interview.InterviewDifficulty.Invalid",
                "Invalid interview difficulty.");

        public static Error GettingOfferInvalid =>
            Error.Validation(
                "Interview.GettingOffer.Invalid",
                "Invalid offer outcome.");

        public static Error SourceInvalid =>
            Error.Validation(
                "Interview.Source.Invalid",
                "Invalid interview source.");

        public static Error HelpingLevelInvalid =>
            Error.Validation(
                "Interview.HelpingLevel.Invalid",
                "Invalid helping level.");

        public static Error StageInvalid =>
            Error.Validation(
                "Interview.Stage.Invalid",
                "Invalid interview stage.");

        public static Error ReadOnly =>
            Error.Conflict(
                "Interview.ReadOnly",
                "Approved interviews cannot be modified.");

        public static Error QuestionAlreadyAdded =>
            Error.Validation(
                "Interview.Question.AlreadyAdded",
                "This interview question was already added.");

        public static Error QuestionNotFound =>
            Error.Validation(
                "Interview.Question.NotFound",
                "Interview question not found.");

        public static Error DurationInvalid =>
            Error.Validation(
        "Interview.Duration.Invalid",
        "Interview duration value must be greater than zero.");

        public static Error DurationUnitInvalid =>
            Error.Validation(
                "Interview.Duration.Unit.Invalid",
                "Invalid interview duration unit.");

        public static Error InterviewDateInvalid =>
            Error.Validation(
                "Interview.Date.Invalid",
                "Interview date (month/year) is invalid.");

        public static Error CannotEditNonPendingInterview =>
            Error.Conflict("Interview.CannotEditNonPending","Only pending interviews can be edited.");
    }
}
