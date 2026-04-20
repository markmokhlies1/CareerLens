using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyMembers.Enums;

namespace CareerLens.Application.Features.CompanyClaimRequests.Dtos
{
    public sealed class CompanyClaimRequestAdminDto : ICompanyClaimRequestResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public ClaimStatus Status { get; set; }
        public string? AdminNote { get; set; }
        public CompanyMemberRole CompanyMemberRole { get; set; }
        public string? CompanyName { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

}
