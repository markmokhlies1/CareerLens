using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Companies
{
    public class UpdateClaimRequestStateRequest
    {
        public ClaimStatus ClaimStatus { get; init; }
    }

    public class UpdateClaimRequestStateRequestValidator: AbstractValidator<UpdateClaimRequestStateRequest>
    {
        public UpdateClaimRequestStateRequestValidator()
        {
            RuleFor(x => x.ClaimStatus)
                .IsInEnum()
                .WithMessage("ClaimStatus is invalid.")
                .NotEqual(ClaimStatus.Pending)
                .WithMessage("Cannot set status back to Pending.");
        }
    }
}
