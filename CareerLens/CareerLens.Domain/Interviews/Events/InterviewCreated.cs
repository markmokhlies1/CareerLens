using CareerLens.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Interviews.Events
{
    public class InterviewCreated : DomainEvent
    {
        public Guid InterviewId { get; }
        public Guid CompanyId { get; }
        public Guid UserId { get; }
        public string JobTitle { get; }

        public InterviewCreated(Guid interviewId,Guid companyId,Guid userId,string jobTitle)
        {
            InterviewId = interviewId;
            CompanyId = companyId;
            UserId = userId;
            JobTitle = jobTitle;
        }
    }
}
