using CareerLens.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Companies.CompanyMembers
{
    public static class CompanyMemberErrors
    {
        public static Error IdRequired =>
            Error.Validation("CompanyMember.IdRequired", "Id is required.");
        public static Error CompanyIdRequired =>
            Error.Validation("CompanyMember.CompanyIdRequired", "CompanyId is required.");

        public static Error UserIdRequired =>
            Error.Validation("CompanyMember.UserIdRequired", "UserId is required.");

        public static readonly Error RoleInvalid =
            Error.Validation("CompanyMember.RoleInvalid", "Company member role is invalid.");
    }
}
