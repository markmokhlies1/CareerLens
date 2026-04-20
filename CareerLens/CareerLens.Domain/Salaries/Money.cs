using Bogus.DataSets;
using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Salaries
{
    public sealed class Money
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        private Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Result<Money> Create(decimal amount, Currency currency)
        {
            if (amount < 0)
            {
                return MoneyErrors.NegativeAmount;
            }
            if (currency == null)
            {
                return MoneyErrors.CurrencyIsNull;
            }

            return new Money(amount, currency);
        }
    }
    public static class MoneyErrors
    {
        public static Error NegativeAmount =>
        Error.Validation(
            "Money.Amount.Negative",
            "Money amount cannot be negative.");

        public static Error CurrencyIsNull =>
            Error.Validation(
                "Money.Currency.Null",
                "Currency must be specified.");
    }
}
