using CareerLens.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Reviews.Events
{
    public sealed class ReviewCreated : DomainEvent
    {
        public Guid ReviewId { get; }
        public Guid CompanyId { get; }
        public Guid UserId { get; }
        public string Headline { get; }

        public ReviewCreated(Guid reviewId, Guid companyId, Guid userId, string headline)
        {
            ReviewId = reviewId;
            CompanyId = companyId;
            UserId = userId;
            Headline = headline;
        }
    }
}
