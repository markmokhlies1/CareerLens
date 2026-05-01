using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.Salaries.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Salaries
{
    public sealed class UpdateSalaryRequest
    {
        public string JobTitle { get; set; } = string.Empty;
        public EmployeeType EmployeeType { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public LengthOfEmployment LengthOfEmployment { get; set; }
        public string Location { get; set; } = string.Empty;
        public MoneyRequest BasePay { get; set; } = default!;
        public SalaryPeriod SalaryPeriod { get; set; }
        public int Year { get; set; }

        public MoneyRequest? Bonus { get; set; }
        public MoneyRequest? Stock { get; set; }
        public MoneyRequest? ProfitSharing { get; set; }
        public MoneyRequest? Tips { get; set; }
        public MoneyRequest? Commission { get; set; }
    }
    public class UpdateSalaryRequestValidator : AbstractValidator<UpdateSalaryRequest>
    {
        public UpdateSalaryRequestValidator()
        {

            RuleFor(x => x.JobTitle)
                .NotEmpty()
                .WithMessage("Job title is required.")
                .MaximumLength(CareerLensConstants.SalaryJobTitleLenght)
                .WithMessage($"Job title must not exceed {CareerLensConstants.SalaryJobTitleLenght} characters.");

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Location is required.")
                .MaximumLength(CareerLensConstants.SalaryLocationLength)
                .WithMessage($"Location must not exceed {CareerLensConstants.SalaryLocationLength} characters.");

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, DateTime.UtcNow.Year + 1)
                .WithMessage($"Year must be between 2000 and {DateTime.UtcNow.Year + 1}.");

            RuleFor(x => x.EmployeeType)
                .IsInEnum()
                .WithMessage("Invalid employee type.");

            RuleFor(x => x.EmploymentStatus)
                .IsInEnum()
                .WithMessage("Invalid employment status.");

            RuleFor(x => x.LengthOfEmployment)
                .IsInEnum()
                .WithMessage("Invalid length of employment.");

            RuleFor(x => x.SalaryPeriod)
                .IsInEnum()
                .WithMessage("Invalid salary period.");

            RuleFor(x => x.BasePay)
                .NotNull()
                .WithMessage("Base pay is required.")
                .SetValidator(new MoneyRequestValidator()!)
                .When(x => x.BasePay is not null);

            RuleFor(x => x.BasePay.Amount)
                .GreaterThanOrEqualTo(CareerLensConstants.SalaryBasePayAmountForYear)
                .WithMessage($"Yearly base pay must be at least {CareerLensConstants.SalaryBasePayAmountForYear}.")
                .When(x => x.SalaryPeriod == SalaryPeriod.Yearly && x.BasePay is not null);

            RuleFor(x => x.BasePay.Amount)
                .GreaterThanOrEqualTo(CareerLensConstants.SalaryBasePayAmountForMonth)
                .WithMessage($"Monthly base pay must be at least {CareerLensConstants.SalaryBasePayAmountForMonth}.")
                .When(x => x.SalaryPeriod == SalaryPeriod.Monthly && x.BasePay is not null);

            RuleFor(x => x.Bonus)
                .SetValidator(new NullableMoneyRequestValidator()!);

            RuleFor(x => x.Stock)
                .SetValidator(new NullableMoneyRequestValidator()!);

            RuleFor(x => x.ProfitSharing)
                .SetValidator(new NullableMoneyRequestValidator()!);

            RuleFor(x => x.Tips)
                .SetValidator(new NullableMoneyRequestValidator()!);

            RuleFor(x => x.Commission)
                .SetValidator(new NullableMoneyRequestValidator()!);
        }
    }
}
