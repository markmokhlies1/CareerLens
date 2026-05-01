using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Reviews
{
    public static class ReviewErrors
    {
        public static Error IdRequired =>
            Error.Validation(
                "Review.Id.Required",
                "Review Id is required.");

        public static Error UserIdRequired =>
            Error.Validation(
                "Review.UserId.Required",
                "User Id is required.");

        public static Error CompanyIdRequired =>
            Error.Validation(
                "Review.CompanyId.Required",
                "Company Id is required.");

        public static Error OverallRatingInvalid =>
            Error.Validation(
                "Review.OverallRating.Invalid",
                "Overall rating must be between 1 and 5.");

        public static Error EmploymentStatusInvalid =>
            Error.Validation(
                "Review.EmploymentStatus.Invalid",
                "Employment status is invalid.");

        public static Error EmployeeTypeInvalid =>
            Error.Validation(
                "Review.EmploymentType.Invalid",
                "Employment Type is invalid.");

        public static Error JobFunctionInvalid =>
            Error.Validation(
                "Review.JobFunction.Invalid",
                "Job function is invalid.");

        public static Error LengthOfEmploymentInvalid =>
            Error.Validation(
                "Review.LengthOfEmployment.Invalid",
                "Length of employment is invalid.");

        public static Error HeadlineRequired =>
            Error.Validation(
                "Review.Headline.Required",
                "Review headline is required.");
        public static Error ProsRequired =>
            Error.Validation(
                "Review.Pros.Required",
                "Review Pros is required.");
        public static Error ConsRequired =>
            Error.Validation(
                "Review.Cons.Required",
                "Review Cons is required.");

        public static Error ProsTooShort =>
            Error.Validation(
                "Review.Pros.TooShort",
                "Pros must contain at least 5 words.");
        public static Error CannotApproveNonPendingReview=>
            Error.Conflict(
                "Review.CannotApproveNonPendingReview",
                "Only Reviews with Pending status can be approved.");
        public static Error CannotRejectNonPendingReview =>
            Error.Conflict(
                "Review.CannotRejectNonPendingReview",
                "Only Reviews with Pending status can be Rejected.");

        public static Error ConsTooShort =>
            Error.Validation(
                "Review.Cons.TooShort",
                "Cons must contain at least 5 words.");

        public static Error CareerOpportunitiesInvalid =>
            Error.Validation("Review.CareerOpportunities.Invalid", "Career opportunities rating must be between 1 and 5.");

        public static Error CompensationAndBenefitsInvalid =>
            Error.Validation("Review.CompensationAndBenefits.Invalid", "Compensation & benefits rating must be between 1 and 5.");

        public static Error CultureAndValuesInvalid =>
            Error.Validation("Review.CultureAndValues.Invalid", "Culture & values rating must be between 1 and 5.");

        public static Error DiversityAndInclusionInvalid =>
            Error.Validation("Review.DiversityAndInclusion.Invalid", "Diversity & inclusion rating must be between 1 and 5.");

        public static Error SeniorManagementInvalid =>
            Error.Validation("Review.SeniorManagement.Invalid", "Senior management rating must be between 1 and 5.");

        public static Error WorkLifeBalanceInvalid =>
            Error.Validation("Review.WorkLifeBalance.Invalid", "Work-life balance rating must be between 1 and 5.");
    }
}
