using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Tests.Common.Constants;
using CareerLens.Tests.Domain.CompanyMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Tests.Common.CompanyMembers
{
    public sealed class CompanyMemberTests
    {
        [Fact]
        public void CreateCompanyMember_ShouldSucceed_WithValidData()
        {
            var id = Guid.NewGuid();
            var companyId = TestConstants.DefaultCompanyId;
            var userId = TestConstants.DefaultUserId;
            const CompanyMemberRole role = CompanyMemberRole.Moderator;

            var result = CompanyMemberFactory.CreateCompanyMember(
                id: id,
                companyId: companyId,
                userId: userId,
                role: role);

            Assert.False(result.IsError);

            var member = result.Value;
            Assert.IsType<CompanyMember>(member);
            Assert.NotNull(member);
            Assert.Equal(id, member.Id);
            Assert.Equal(companyId, member.CompanyId);
            Assert.Equal(userId, member.UserId);
            Assert.Equal(role, member.Role);
        }

        [Fact]
        public void CreateCompanyMember_ShouldFail_WhenIdIsEmpty()
        {
            var result = CompanyMemberFactory.CreateCompanyMember(id: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyMemberErrors.IdRequired, result.Errors[0]);
        }

        [Fact]
        public void CreateCompanyMember_ShouldFail_WhenCompanyIdIsEmpty()
        {
            var result = CompanyMemberFactory.CreateCompanyMember(companyId: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyMemberErrors.CompanyIdRequired, result.Errors[0]);
        }

        [Fact]
        public void CreateCompanyMember_ShouldFail_WhenUserIdIsEmpty()
        {
            var result = CompanyMemberFactory.CreateCompanyMember(userId: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyMemberErrors.UserIdRequired, result.Errors[0]);
        }

        [Fact]
        public void CreateCompanyMember_ShouldFail_WhenRoleIsInvalid()
        {
            var result = CompanyMemberFactory.CreateCompanyMember(role: (CompanyMemberRole)999);

            Assert.True(result.IsError);
            Assert.Equal(CompanyMemberErrors.RoleInvalid, result.Errors[0]);
        }

        [Theory]
        [InlineData(CompanyMemberRole.Moderator)]
        [InlineData(CompanyMemberRole.Hr)]
        public void CreateCompanyMember_ShouldSucceed_WithAllValidRoles(CompanyMemberRole role)
        {
            var result = CompanyMemberFactory.CreateCompanyMember(role: role);

            Assert.False(result.IsError);
            Assert.Equal(role, result.Value.Role);
        }
    }
}
