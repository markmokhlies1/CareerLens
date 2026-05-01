using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Tests.Common.CompanyClaimRequestes;
using CareerLens.Tests.Common.Constants;

namespace CareerLens.Tests.Domain.CompanyClaimRequestes
{
    public sealed class CompanyClaimRequestTests
    {
        
        [Fact]
        public void CreateCompanyClaimRequest_ShouldSucceed_WithValidData()
        {
            var id = Guid.NewGuid();
            var companyId = TestConstants.DefaultCompanyId;
            var userId = TestConstants.DefaultUserId;
            const string adminNote = "Requesting moderator access for our company page.";
            const CompanyMemberRole memberRole = CompanyMemberRole.Moderator;

            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest(
                id: id,
                companyId: companyId,
                userId: userId,
                adminNote: adminNote,
                memberRole: memberRole);

            Assert.False(result.IsError);

            var claimRequest = result.Value;
            Assert.IsType<CompanyClaimRequest>(claimRequest);
            Assert.NotNull(claimRequest);
            Assert.Equal(id, claimRequest.Id);
            Assert.Equal(companyId, claimRequest.CompanyId);
            Assert.Equal(userId, claimRequest.UserId);
            Assert.Equal(adminNote, claimRequest.AdminNote);
            Assert.Equal(memberRole, claimRequest.CompanyMemberRole);
            Assert.Equal(ClaimStatus.Pending, claimRequest.Status);
        }

        [Fact]
        public void CreateCompanyClaimRequest_ShouldSetStatusToPendingByDefault()
        {
            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest();

            Assert.False(result.IsError);
            Assert.Equal(ClaimStatus.Pending, result.Value.Status);
        }

        [Fact]
        public void CreateCompanyClaimRequest_ShouldFail_WhenIdIsEmpty()
        {
            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest(id: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.IdRequired, result.Errors[0]);
        }


        [Fact]
        public void CreateCompanyClaimRequest_ShouldFail_WhenCompanyIdIsEmpty()
        {
            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest(companyId: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.CompanyIdRequired, result.Errors[0]);
        }

        [Fact]
        public void CreateCompanyClaimRequest_ShouldFail_WhenUserIdIsEmpty()
        {
            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest(userId: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.UserIdRequired, result.Errors[0]);
        }

        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateCompanyClaimRequest_ShouldFail_WhenAdminNoteIsInvalid(string? adminNote)
        {
            var result = CompanyClaimRequest.Create(
                id: Guid.NewGuid(),
                companyId: TestConstants.DefaultCompanyId,
                userId: TestConstants.DefaultUserId,
                adminNote: adminNote!,
                companyMemberRole: CompanyMemberRole.Moderator);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.AdmainNoteRequired, result.Errors[0]);
        }

        [Fact]
        public void CreateCompanyClaimRequest_ShouldFail_WhenCompanyMemberRoleIsInvalid()
        {
            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest(
                memberRole: (CompanyMemberRole)999);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.CompanyMemberRoleInValied, result.Errors[0]);
        }

        [Theory]
        [InlineData(CompanyMemberRole.Moderator)]
        [InlineData(CompanyMemberRole.Hr)]
        public void CreateCompanyClaimRequest_ShouldSucceed_WithAllValidRoles(CompanyMemberRole role)
        {
            var result = CompanyClaimRequestFactory.CreateCompanyClaimRequest(memberRole: role);

            Assert.False(result.IsError);
            Assert.Equal(role, result.Value.CompanyMemberRole);
        }


        [Fact]
        public void Update_ShouldSucceed_WithValidData()
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;
            const string newAdminNote = "Updated note for HR access request.";
            const CompanyMemberRole newRole = CompanyMemberRole.Hr;

            var result = claimRequest.Update(newAdminNote, newRole);

            Assert.False(result.IsError);
            Assert.Equal(newAdminNote, claimRequest.AdminNote);
            Assert.Equal(newRole, claimRequest.CompanyMemberRole);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Update_ShouldFail_WhenAdminNoteIsInvalid(string? adminNote)
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;

            var result = claimRequest.Update(adminNote!, CompanyMemberRole.Moderator);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.AdmainNoteRequired, result.Errors[0]);
        }


        [Fact]
        public void Update_ShouldFail_WhenCompanyMemberRoleIsInvalid()
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;

            var result = claimRequest.Update("Valid note.", (CompanyMemberRole)999);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.CompanyMemberRoleInValied, result.Errors[0]);
        }


        [Theory]
        [InlineData(ClaimStatus.Approved)]
        [InlineData(ClaimStatus.Rejected)]
        public void Update_ShouldFail_WhenStatusIsNotPending(ClaimStatus status)
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;
            claimRequest.UpdateState(status); 

            var result = claimRequest.Update("Valid note.", CompanyMemberRole.Moderator);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.NotPending, result.Errors[0]);
        }


        [Theory]
        [InlineData(ClaimStatus.Approved)]
        [InlineData(ClaimStatus.Rejected)]
        public void UpdateState_ShouldSucceed_WithValidTransition(ClaimStatus newStatus)
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;

            var result = claimRequest.UpdateState(newStatus);

            Assert.False(result.IsError);
            Assert.Equal(newStatus, claimRequest.Status);
        }
        [Fact]
        public void UpdateState_ShouldFail_WhenNewStatusIsPending()
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;

            var result = claimRequest.UpdateState(ClaimStatus.Pending);

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.InvalidStatusTransition, result.Errors[0]);
        }

        [Theory]
        [InlineData(ClaimStatus.Approved)]
        [InlineData(ClaimStatus.Rejected)]
        public void UpdateState_ShouldFail_WhenStatusIsAlreadyNotPending(ClaimStatus status)
        {
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value;
            claimRequest.UpdateState(status); // move out of pending

            var result = claimRequest.UpdateState(ClaimStatus.Approved); // try again

            Assert.True(result.IsError);
            Assert.Equal(CompanyClaimRequestErrors.NotPending, result.Errors[0]);
        }
    }
}
