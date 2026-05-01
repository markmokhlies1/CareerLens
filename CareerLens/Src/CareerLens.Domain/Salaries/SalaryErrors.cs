using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Salaries
{
    public static class SalaryErrors
    {
        public static Error IdRequired =>
        Error.Validation(
            "Salary.Id.Required",
            "Salary Id is required.");

        public static Error UserIdRequired =>
            Error.Validation(
                "Salary.UserId.Required",
                "User Id is required.");

        public static Error CompanyIdRequired =>
            Error.Validation(
                "Salary.CompanyId.Required",
                "Company Id is required.");

        public static Error JobTitleRequired =>
            Error.Validation(
                "Salary.JobTitle.Required",
                "Job title is required.");

        public static Error JobTitleTooLong =>
            Error.Validation(
                "Salary.JobTitle.TooLong",
                "Job title must not exceed 100 characters.");

        public static Error EmployeeTypeInvalid =>
            Error.Validation(
                "Salary.EmployeeType.Invalid",
                "Employee type is invalid.");

        public static Error EmploymentStatusInvalid =>
            Error.Validation(
                "Salary.EmploymentStatus.Invalid",
                "Employment status is invalid.");

        public static Error LengthOfEmploymentInvalid =>
            Error.Validation(
                "Salary.LengthOfEmployment.Invalid",
                "Length of employment is invalid.");

        public static Error SalaryPeriodInvalid =>
            Error.Validation(
                "Salary.Period.Invalid",
                "Salary period is invalid.");

        public static Error BasePayUnrealistic =>
            Error.Validation(
                "Salary.BasePay.Unrealistic",
                "Base pay amount is unrealistically low for the selected salary period.");

        public static Error InvalidYear =>
            Error.Validation(
                "Salary.Year.Invalid",
                "Salary year must be between 2000 and next year.");

        public static Error LocationRequired =>
            Error.Validation(
                "Salary.Location.Required",
                "Location is required.");

        public static Error LocationTooLong =>
            Error.Validation(
                "Salary.Location.TooLong",
                "Location must not exceed 100 characters.");

        public static Error SalaryAlreadyApproved =>
            Error.Conflict(
                "Salary.AlreadyApproved",
                "Salary has already been approved.");

        public static Error SalaryAlreadyRejected =>
            Error.Conflict(
                "Salary.AlreadyRejected",
                "Salary has already been rejected.");

        public static Error CannotApproveRejectedSalary =>
            Error.Conflict(
                "Salary.CannotApproveRejected",
                "Rejected salary cannot be approved.");

        public static Error CannotRejectApprovedSalary =>
            Error.Conflict(
                "Salary.CannotRejectApproved",
                "Approved salary cannot be rejected.");

        public static Error CannotSetStatusToPending =>
        Error.Conflict(
            code: "Salary.CannotSetStatusToPending",
            description: "Cannot set salary status back to Pending.");

        public static Error StatusAlreadySet =>
            Error.Conflict(
                code: "Salary.StatusAlreadySet",
                description: "Salary is already in the requested status.");

        public static Error CannotModifyApprovedSalary =>
            Error.Conflict(
                code: "Salary.CannotModifyApproved",
                description: "Cannot modify an approved salary.");

        public static Error CannotModifyRejectedSalary =>
            Error.Conflict(
                code: "Salary.CannotModifyRejected",
                description: "Cannot modify a rejected salary.");

        public static Error InvalidStatusTransition =>
            Error.Conflict(
                code: "Salary.InvalidStatusTransition",
                description: "This status transition is not allowed.");
    }
}
