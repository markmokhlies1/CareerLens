using CareerLens.Contracts.Common;
using CareerLens.Domain.Common.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Companies
{
    public class CreateCompanyRequest
    {
        public string Name { get; init; } = default!;
        public string Industry { get; init; } = default!;
        public string Location { get; init; } = default!;
        public string Website { get; init; } = default!;
        public string Description { get; init; } = default!;
        public int FoundedYear { get; init; }
        public CompanySize Size { get; init; }
    }
    public class CreateCompanyRequestValidator: AbstractValidator<CreateCompanyRequest>
    {
        public CreateCompanyRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Company name is required.");

            RuleFor(x => x.Industry)
                .NotEmpty()
                    .WithMessage("Industry is required.");

            RuleFor(x => x.Location)
                .NotEmpty()
                    .WithMessage("Location is required.");

            RuleFor(x => x.Website)
                .NotEmpty()
                    .WithMessage("Website is required.")
                .Must(BeAValidUrl)
                    .WithMessage("Website must be a valid URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.Website));

            RuleFor(x => x.Description)
                .NotEmpty()
                    .WithMessage("Description is required.")
                .MaximumLength(CareerLensConstants.CompanyDescriptionMaxLenght)
                    .WithMessage($"Description must not exceed " +
                                 $"{CareerLensConstants.CompanyDescriptionMaxLenght} characters.");

            RuleFor(x => x.FoundedYear)
                .GreaterThanOrEqualTo(1800)
                    .WithMessage("Founded year must be 1800 or later.")
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                    .WithMessage("Founded year cannot be in the future.");

            RuleFor(x => x.Size)
                .IsInEnum()
                    .WithMessage("Invalid company size.");
        }

        private static bool BeAValidUrl(string url)
            => Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
