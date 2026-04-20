using CareerLens.Domain.Jobs.Enums;

namespace CareerLens.Application.Features.Jobs.Dtos
{
    public class JobBasicDto : IJobResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public WorkplaceType WorkplaceType { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public PayPeriod? PayPeriod { get; set; }
        public string? ApplyUrl { get; set; }
    }
}
