using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Jobs.Enums;

namespace CareerLens.Domain.Jobs
{
    public static class JobErrors
    {
            public static Error IdRequired =>
                Error.Validation("Job.Id.Required", "Job Id is required.");

            public static Error CompanyIdRequired =>
                Error.Validation("Job.CompanyId.Required", "Company Id is required.");

            public static Error PostedByUserRequired =>
                Error.Validation("Job.PostedByUser.Required", "Posting user is required.");

            public static Error TitleRequired =>
                Error.Validation("Job.Title.Required", "Job title is required.");

            public static Error DescriptionRequired =>
                Error.Validation("Job.Description.Required", "Job description is required.");

            public static Error LocationRequired =>
                Error.Validation("Job.Location.Required", "Job location is required.");

            public static Error EmploymentTypeInvalid =>
                Error.Validation("Job.EmploymentType.Invalid", "Invalid employment type.");

            public static Error WorkplaceTypeInvalid =>
                Error.Validation("Job.WorkplaceType.Invalid", "Invalid workplace type.");

            public static Error ExperienceLevelInvalid =>
                Error.Validation("Job.ExperienceLevel.Invalid", "Invalid experience level.");

            public static Error PayPeriodInvalid =>
                Error.Validation("Job.PayPeriod.Invalid", "Invalid pay period.");

            public static Error InvalidSalaryRange =>
                Error.Validation(
                    "Job.Salary.Invalid",
                    "Minimum salary must be less than or equal to maximum salary.");

            public static Error ApplyUrlRequired =>
                Error.Validation("Job.ApplyUrl.Required", "Apply URL is required.");

            public static Error ApplyUrlInvalid =>
                Error.Validation(
                    "Job.ApplyUrl.Invalid",
                    "Apply URL must be a valid absolute URL.");
            public static Error JobAlreadyDrafted =>
                Error.Conflict("Job.AlreadyDrafted", "Job is already in Draft state.");
            public static Error JobAlreadyPublished =>
                Error.Conflict("Job.AlreadyPublished", "Job is already Published.");

            public static Error JobAlreadyClosed =>
                Error.Conflict("Job.AlreadyClosed", "Job is already Closed.");

            public static Error CannotModifyClosedJob =>
                Error.Conflict("Job.AlreadyClosed", "Job is already Closed Cannot Modify.");
            public static Error JobNotPublished =>
                Error.Conflict("Job.NotPublished", "Job is not Published To Close It.");

        public static Error StatusAlreadySet =>
                Error.Conflict("Job.StatusAlreadySet", $"Job is already in this status.");

        public static readonly Error InvalidStatusTransition =
                Error.Conflict("Job.InvalidStatusTransition", "This status transition is not allowed.");

    }
}
