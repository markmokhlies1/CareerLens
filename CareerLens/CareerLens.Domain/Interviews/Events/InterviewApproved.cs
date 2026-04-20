using CareerLens.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Interviews.Events
{
    public sealed class InterviewApproved : DomainEvent
    {
        public Guid InterviewId { get; }
        public Guid UserId { get; }
        public string JobTitle { get; }

        public InterviewApproved(Guid interviewId, Guid userId, string jobTitle)
        {
            InterviewId = interviewId;
            UserId = userId;
            JobTitle = jobTitle;
        }
    }
}
