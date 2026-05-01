using CareerLens.Domain.Reviews.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Reviews
{
    public class UpdateReviewStatusRequest
    {
        public ReviewStatus Status { get; set; }
    }

    public sealed class UpdateReviewStatusRequestValidator
    : AbstractValidator<UpdateReviewStatusRequest>
    {
        private static readonly ReviewStatus[] AllowedStatuses =
        [
            ReviewStatus.Approved,
            ReviewStatus.Rejected
        ];

        public UpdateReviewStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid review status.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage("Status must be Approved or Rejected.");
        }
    }
}
