using CareerLens.Domain.Common;

namespace CareerLens.Domain.Reviews.Events
{
    public sealed class ReviewRejected : DomainEvent
    {
        public Guid ReviewId { get; }
        public Guid UserId { get; }
        public string Headline { get; }

        public ReviewRejected(Guid reviewId, Guid userId, string headline)
        {
            ReviewId = reviewId;
            UserId = userId;
            Headline = headline;
        }
    }
}
