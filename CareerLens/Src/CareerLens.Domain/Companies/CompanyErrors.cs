using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Companies
{
    public static class CompanyErrors
    {
        public static Error IdRequired =>
            Error.Validation("Company.Id.Required", "Company Id is required.");

        public static Error NameRequired =>
            Error.Validation("Company.Name.Required", "Company name is required.");

        public static Error IndustryRequired =>
            Error.Validation("Company.Industry.Required", "Industry is required.");

        public static Error LocationRequired =>
            Error.Validation("Company.Location.Required", "Location is required.");
         
        public static Error SizeInvalid =>
            Error.Validation("Company.Size.Invalid", "Invalid company size.");

        public static Error WebsiteRequired =>
            Error.Validation("Company.Website.Required", "Website URL is required.");

        public static Error WebsiteInvalid =>
            Error.Validation("Company.Website.Invalid", "Website URL is invalid.");

        public static Error FoundedYearInvalid =>
            Error.Validation("Company.FoundedYear.Invalid", "Founded year is invalid.");

        public static Error DescriptionRequired =>
            Error.Validation("Company.DescriptionRequired", "Description is required.");

        public static readonly Error DescriptionTooLong =
            Error.Validation("Company.DescriptionTooLong", $"Description is too long. Maximum length is {CareerLensConstants.CompanyDescriptionMaxLenght} characters.");

        public static Error AlreadyClaimed =>
            Error.Conflict("Company.AlreadyClaimed","This company has already been claimed.");

        public static Error NameAlreadyExists =>
            Error.Conflict("Company.AlreadyExists", "This company has already been created.");
    }
}
