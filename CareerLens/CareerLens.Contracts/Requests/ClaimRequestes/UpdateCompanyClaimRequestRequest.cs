using CareerLens.Domain.Companies.CompanyMembers.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.ClaimRequestes
{
    public class UpdateCompanyClaimRequestRequest
    {
        public string AdminNote { get; init; } = default!;
        public CompanyMemberRole CompanyMemberRole { get; init; }
    }

    public class UpdateCompanyClaimRequestRequestValidator
        : AbstractValidator<UpdateCompanyClaimRequestRequest>
    {
        public UpdateCompanyClaimRequestRequestValidator()
        {
            RuleFor(x => x.AdminNote)
                .NotEmpty()
                .WithMessage("AdminNote is required.")
                .MaximumLength(2000)
                .WithMessage("AdminNote is too long. Maximum length is 2000 characters.");

            RuleFor(x => x.CompanyMemberRole)
                .IsInEnum()
                .WithMessage("CompanyMemberRole is invalid.");
        }
    }
}
