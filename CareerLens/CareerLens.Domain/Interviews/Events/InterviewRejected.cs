using CareerLens.Domain.Common;

namespace CareerLens.Domain.Interviews.Events
{
    public sealed class InterviewRejected: DomainEvent
    {
        public Guid InterviewId { get; }
        public Guid UserId { get; }
        public string JobTitle { get; }

        public InterviewRejected(Guid interviewId, Guid userId, string jobTitle)
        {
            InterviewId = interviewId;
            UserId = userId;
            JobTitle = jobTitle;
        }
    }
}
