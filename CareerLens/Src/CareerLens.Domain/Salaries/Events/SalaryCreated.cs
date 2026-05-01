using CareerLens.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Salaries.Events
{
    public sealed class SalaryCreated : DomainEvent
    {
        public Guid SalaryId { get; }
        public Guid CompanyId { get; }
        public Guid UserId { get; }
        public string JobTitle { get; }

        public SalaryCreated(Guid salaryId, Guid companyId, Guid userId, string jobTitle)
        {
            SalaryId = salaryId;
            CompanyId = companyId;
            UserId = userId;
            JobTitle = jobTitle;
        }
    }
}
