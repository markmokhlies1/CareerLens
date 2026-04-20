using CareerLens.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Companies.CompanyClaimRequests
{
    public static class CompanyClaimRequestErrors
    {
        public static Error IdRequired =>
        Error.Validation("CompanyClaimRequest.IdRequired", "Id is required.");

        public static  Error CompanyIdRequired =>
            Error.Validation("CompanyClaimRequest.CompanyIdRequired", "CompanyId is required.");

        public static  Error UserIdRequired =>
            Error.Validation("CompanyClaimRequest.UserIdRequired", "UserId is required.");

        public static  Error AdmainNoteRequired =>
            Error.Validation("CompanyClaimRequest.AdminNoteRequired", "Admin note is required.");

        public static  Error CompanyMemberRoleInValied =>
            Error.Validation("CompanyClaimRequest.CompanyMemberRoleInvalid", "Company member role is invalid.");

        public static  Error NotPending =>
            Error.Conflict("CompanyClaimRequest.NotPending", "Only pending claim requests can be approved or rejected.");
        public static Error InvalidStatusTransition =>
            Error.Conflict("CompanyClaimRequest.AlreadyPending", "CompanyClaimRequest is Already Pending.");
    }
}
