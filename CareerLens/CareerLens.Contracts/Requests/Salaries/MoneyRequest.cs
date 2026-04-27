using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Salaries
{
    public sealed class MoneyRequest
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty; 
    }

    public sealed class MoneyRequestValidator : AbstractValidator<MoneyRequest>
    {
        public MoneyRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Amount cannot be negative.");

            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithMessage("Currency code is required.")
                .Length(3)
                .WithMessage("Currency code must be exactly 3 characters (e.g. USD, EUR).")
                .Matches("^[A-Z]{3}$")
                .WithMessage("Currency code must be 3 uppercase letters (e.g. USD, EUR).");
        }
    }

    public sealed class NullableMoneyRequestValidator : AbstractValidator<MoneyRequest?>
    {
        public NullableMoneyRequestValidator()
        {
            When(x => x is not null, () =>
            {
                RuleFor(x => x!.Amount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Amount cannot be negative.");

                RuleFor(x => x!.CurrencyCode)
                    .NotEmpty()
                    .WithMessage("Currency code is required.")
                    .Length(3)
                    .WithMessage("Currency code must be exactly 3 characters (e.g. USD, EUR).")
                    .Matches("^[A-Z]{3}$")
                    .WithMessage("Currency code must be 3 uppercase letters (e.g. USD, EUR).");
            });
        }
    }
}
