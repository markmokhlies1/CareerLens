using CareerLens.Domain.Common;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Companies.CompanyClaimRequests.Events
{
    public sealed class CompanyClaimRequestCreated : DomainEvent
    {
        public Guid ClaimRequestId { get; }
        public Guid CompanyId { get; }
        public Guid UserId { get; }
        public string CompanyName { get; }
        public CompanyMemberRole RequestedRole { get; }

        public CompanyClaimRequestCreated(Guid claimRequestId, Guid companyId, Guid userId, string companyName, CompanyMemberRole requestedRole)
        {
            ClaimRequestId = claimRequestId;
            CompanyId = companyId;
            UserId = userId;
            CompanyName = companyName;
            RequestedRole = requestedRole;
        }
    }
    public sealed class CompanyClaimRequestApproved : DomainEvent
    {
        public Guid ClaimRequestId { get; }
        public Guid CompanyId { get; }
        public Guid UserId { get; }
        public string CompanyName { get; }
        public CompanyMemberRole RequestedRole { get; }

        public CompanyClaimRequestApproved(Guid claimRequestId, Guid companyId, Guid userId, string companyName, CompanyMemberRole requestedRole)
        {
            ClaimRequestId = claimRequestId;
            CompanyId = companyId;
            UserId = userId;
            CompanyName = companyName;
            RequestedRole = requestedRole;
        }
    } 
    public sealed class CompanyClaimRequestRejected : DomainEvent
    {
        public Guid ClaimRequestId { get; }
        public Guid CompanyId { get; }
        public Guid UserId { get; }
        public string CompanyName { get; }
        public CompanyMemberRole RequestedRole { get; }

        public CompanyClaimRequestRejected(Guid claimRequestId, Guid companyId, Guid userId, string companyName, CompanyMemberRole requestedRole)
        {
            ClaimRequestId = claimRequestId;
            CompanyId = companyId;
            UserId = userId;
            CompanyName = companyName;
            RequestedRole = requestedRole;
        }
    }
}
