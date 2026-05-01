using CareerLens.Domain.Salaries;

namespace CareerLens.Application.Features.Salaries.Dtos
{
    public sealed record MoneyDto(decimal Amount, Currency Currency);
}
