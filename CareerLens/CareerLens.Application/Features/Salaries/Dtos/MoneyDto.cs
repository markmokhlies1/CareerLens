using Bogus.DataSets;

namespace CareerLens.Application.Features.Salaries.Dtos
{
    public sealed record MoneyDto(decimal Amount, Currency Currency);
}
