using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Domain.Salaries;

namespace CareerLens.Application.Features.Salaries.Mappers
{
    public static class SalaryMappings
    {
        private static MoneyDto? ToMoneyDto(Money? money)
        {
            return money is null
                ? null
                : new MoneyDto(money.Amount, money.Currency);
        }

        public static SalaryBasicDto ToBasicDto(this Salary entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new SalaryBasicDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                JobTitle = entity.JobTitle,
                EmployeeType = entity.EmployeeType,
                EmploymentStatus = entity.EmploymentStatus,
                LengthOfEmployment = entity.LengthOfEmployment,
                Location = entity.Location,
                BasePay = ToMoneyDto(entity.BasePay),
                Bonus = ToMoneyDto(entity.Bonus),
                Stock = ToMoneyDto(entity.Stock),
                ProfitSharing = ToMoneyDto(entity.ProfitSharing),
                Tips = ToMoneyDto(entity.Tips),
                Commission = ToMoneyDto(entity.Commission),
                SalaryPeriod = entity.SalaryPeriod,
                Year = entity.Year
            };
        }

        public static SalaryEmployerDto ToEmployerDto(this Salary entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new SalaryEmployerDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CompanyId = entity.CompanyId,
                JobTitle = entity.JobTitle,
                EmployeeType = entity.EmployeeType,
                EmploymentStatus = entity.EmploymentStatus,
                LengthOfEmployment = entity.LengthOfEmployment,
                Location = entity.Location,
                BasePay = ToMoneyDto(entity.BasePay),
                Bonus = ToMoneyDto(entity.Bonus),
                Stock = ToMoneyDto(entity.Stock),
                ProfitSharing = ToMoneyDto(entity.ProfitSharing),
                Tips = ToMoneyDto(entity.Tips),
                Commission = ToMoneyDto(entity.Commission),
                SalaryPeriod = entity.SalaryPeriod,
                Year = entity.Year,
                Status = entity.Status
            };
        }

        public static List<ISalaryResponse> ToBasicDtos(this IEnumerable<Salary> entities)
        {
            return [.. entities.Select(e => e.ToBasicDto())];
        }

        public static List<ISalaryResponse> ToEmployerDtos(this IEnumerable<Salary> entities)
        {
            return [.. entities.Select(e => e.ToEmployerDto())];
        }
    }

}
