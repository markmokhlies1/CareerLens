using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Tests.Common.Constants;

namespace CareerLens.Tests.Common.CompanyClaimRequestes
{
    public static class CompanyClaimRequestFactory
    {
        public static Result<CompanyClaimRequest> CreateCompanyClaimRequest(
            Guid? id = null,
            Guid? companyId = null,
            Guid? userId = null,
            string? adminNote = null,
            CompanyMemberRole? memberRole = null)
        {
            return CompanyClaimRequest.Create(
                id: id ?? Guid.NewGuid(),
                companyId: companyId ?? TestConstants.DefaultCompanyId,
                userId: userId ?? TestConstants.DefaultUserId,
                adminNote: adminNote ?? "Requesting moderator access for our company page.",
                companyMemberRole: memberRole ?? CompanyMemberRole.Moderator
            );
        }
    }
}
