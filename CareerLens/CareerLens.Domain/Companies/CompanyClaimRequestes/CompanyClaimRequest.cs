using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.DomainUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Companies.CompanyClaimRequests
{
    public class CompanyClaimRequest : AuditableEntity
    {
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;
        public Guid UserId { get; private set; }   
        public User? User { get; private set; } = null!; 
        public ClaimStatus Status { get; private set; }
        public string? AdminNote { get; private set; }
        public CompanyMemberRole  CompanyMemberRole { get; private set; }

        private CompanyClaimRequest() 
        {
        }
        private CompanyClaimRequest(Guid id, Guid companyId, Guid userId, string adminNote, CompanyMemberRole companyMemberRole) : base(id)
        {
            CompanyId = companyId;
            UserId = userId;
            AdminNote = adminNote;
            CompanyMemberRole = companyMemberRole;
            Status = ClaimStatus.Pending;
        }
        public static Result<CompanyClaimRequest> Create(Guid id,Guid companyId, Guid userId, string adminNote,CompanyMemberRole companyMemberRole)
        {
            if (id == Guid.Empty)
            {
                return CompanyClaimRequestErrors.IdRequired;
            }

            if (companyId == Guid.Empty)
            {
                return CompanyClaimRequestErrors.CompanyIdRequired;
            }

            if (userId == Guid.Empty)
            {
                return CompanyClaimRequestErrors.UserIdRequired;
            }

            if(string.IsNullOrEmpty(adminNote))
            {
                return CompanyClaimRequestErrors.AdmainNoteRequired;
            }

            if(!Enum.IsDefined(companyMemberRole))
            {
                return CompanyClaimRequestErrors.CompanyMemberRoleInValied;
            }

            return new CompanyClaimRequest(id, companyId, userId, adminNote, companyMemberRole);
        }

        public Result<Updated> Update(string adminNote, CompanyMemberRole companyMemberRole)
        {
            if (Status is not ClaimStatus.Pending)
            {
                return CompanyClaimRequestErrors.NotPending;
            }

            if (string.IsNullOrEmpty(adminNote))
            {
                return CompanyClaimRequestErrors.AdmainNoteRequired;
            }

            if (!Enum.IsDefined(companyMemberRole))
            {
                return CompanyClaimRequestErrors.CompanyMemberRoleInValied;
            }

            AdminNote = adminNote;
            CompanyMemberRole = companyMemberRole;

            return Result.Updated;
        }

        public Result<Updated> UpdateState(ClaimStatus newClaimStatus)
        {
            if (Status is not ClaimStatus.Pending)
            {
                return CompanyClaimRequestErrors.NotPending;
            }

            if (newClaimStatus == ClaimStatus.Pending)
            {
                return CompanyClaimRequestErrors.InvalidStatusTransition;
            }

            Status = newClaimStatus;

            return Result.Updated;
        }

    }
}
