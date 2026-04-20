using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.Salaries.Enums;

namespace CareerLens.Application.Features.Salaries.Dtos
{
    public class SalaryBasicDto : ISalaryResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public LengthOfEmployment LengthOfEmployment { get; set; }
        public string? Location { get; set; }
        public MoneyDto? BasePay { get; set; }
        public MoneyDto? Bonus { get; set; }
        public MoneyDto? Stock { get; set; }
        public MoneyDto? ProfitSharing { get; set; }
        public MoneyDto? Tips { get; set; }
        public MoneyDto? Commission { get; set; }
        public SalaryPeriod SalaryPeriod { get; set; }
        public int Year { get; set; }
    }
}
