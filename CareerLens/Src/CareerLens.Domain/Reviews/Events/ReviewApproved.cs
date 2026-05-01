using CareerLens.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Reviews.Events
{
    public sealed class ReviewApproved : DomainEvent
    {
        public Guid ReviewId { get; }
        public Guid UserId { get; }
        public string Headline { get; }

        public ReviewApproved(Guid reviewId, Guid userId, string headline)
        {
            ReviewId = reviewId;
            UserId = userId;
            Headline = headline;
        }
    }
}
