using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Companies.CompanyMembers
{
    public sealed class CompanyMember : AuditableEntity
    {
        public Guid CompanyId { get; private set; }
        public Guid UserId { get; private set; }
        public CompanyMemberRole Role { get; private set; }
        private CompanyMember()
        {
        }
        private CompanyMember(Guid id, Guid companyId, Guid userId, CompanyMemberRole role)
            :base(id)
        {
            CompanyId = companyId;
            UserId = userId;
            Role = role;
        }
        public static Result<CompanyMember> Create(Guid id, Guid companyId, Guid userId, CompanyMemberRole role)
        {
            if (id == Guid.Empty)
            {
                return CompanyMemberErrors.IdRequired;
            }
            if (companyId == Guid.Empty)
            {
                return CompanyMemberErrors.CompanyIdRequired;
            }

            if (userId == Guid.Empty)
            {
                return CompanyMemberErrors.UserIdRequired;
            }

            if(!Enum.IsDefined(role))
            {
                return CompanyMemberErrors.RoleInvalid;
            }

            return new CompanyMember(id,companyId, userId, role);
        }
        
    }
}
