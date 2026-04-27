using CareerLens.Domain.Salaries.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Salaries
{
    public sealed class UpdateSalaryStatusRequest
    {
        public SalaryStatus Status { get; set; }
    }
    public sealed class UpdateSalaryStatusRequestValidator
    : AbstractValidator<UpdateSalaryStatusRequest>
    {
        private static readonly SalaryStatus[] AllowedStatuses =
        [
            SalaryStatus.Approved,
        SalaryStatus.Rejected
        ];

        public UpdateSalaryStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid salary status.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage("Status must be Approved or Rejected.");
        }
    }
}
