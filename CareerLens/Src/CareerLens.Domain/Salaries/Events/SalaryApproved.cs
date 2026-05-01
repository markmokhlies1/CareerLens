using CareerLens.Domain.Common;

namespace CareerLens.Domain.Salaries.Events
{
    public sealed class SalaryApproved: DomainEvent
    {
        public Guid SalaryId { get; }
        public Guid UserId { get; }
        public string JobTitle { get; }

        public SalaryApproved(Guid salaryId, Guid userId, string jobTitle)
        {
            SalaryId = salaryId;
            UserId = userId;
            JobTitle = jobTitle;
        }
    }
}
