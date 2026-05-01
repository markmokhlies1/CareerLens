using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Tests.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Tests.Domain.CompanyMembers
{
    public static class CompanyMemberFactory
    {
        public static Result<CompanyMember> CreateCompanyMember(
            Guid? id = null,
            Guid? companyId = null,
            Guid? userId = null,
            CompanyMemberRole? role = null)
        {
            return CompanyMember.Create(
                id: id ?? Guid.NewGuid(),
                companyId: companyId ?? TestConstants.DefaultCompanyId,
                userId: userId ?? TestConstants.DefaultUserId,
                role: role ?? CompanyMemberRole.Moderator
            );
        }
    }
}
