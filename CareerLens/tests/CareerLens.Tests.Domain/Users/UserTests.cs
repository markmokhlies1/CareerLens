using CareerLens.Domain.DomainUsers;
using CareerLens.Domain.DomainUsers.Enums;
using CareerLens.Tests.Common.Users;

namespace CareerLens.Tests.Domain.Users
{
    public sealed class UserTests
    {
        [Fact]
        public void CreateUser_ShouldSucceed_WithValidData()
        {
            var id = Guid.NewGuid();
            const string firstName = "John";
            const string lastName = "Doe";
            const string email = "john.doe@example.com";
            const Role role = Role.Employee;

            var result = UserFactory.CreateUser(
                id: id,
                firstName: firstName,
                lastName: lastName,
                email: email,
                role: role);

            Assert.False(result.IsError);

            var user = result.Value;
            Assert.IsType<User>(user);
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(email, user.Email);
            Assert.Equal(role, user.Role);
        }

        [Fact]
        public void CreateUser_ShouldFail_WhenIdIsEmpty()
        {
            var result = UserFactory.CreateUser(id: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.IdRequired, result.Errors[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateUser_ShouldFail_WhenFirstNameIsInvalid(string? firstName)
        {
            var result = User.Create(
                id: Guid.NewGuid(),
                firstName: firstName!,
                lastName: "Doe",
                email: "john.doe@example.com",
                role: Role.Employee);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.FirstNameRequired, result.Errors[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateUser_ShouldFail_WhenLastNameIsInvalid(string? lastName)
        {
            var result = User.Create(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: lastName!,
                email: "john.doe@example.com",
                role: Role.Employee);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.LastNameRequired, result.Errors[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateUser_ShouldFail_WhenEmailIsEmpty(string? email)
        {
            var result = User.Create(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                email: email!,
                role: Role.Employee);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.EmailRequired, result.Errors[0]);
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("missing@")]
        [InlineData("@nodomain.com")]
        [InlineData("plaintext")]
        [InlineData("missing.domain@")]
        public void CreateUser_ShouldFail_WhenEmailIsInvalid(string email)
        {
            var result = UserFactory.CreateUser(email: email);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.EmailInvalid, result.Errors[0]);
        }

        [Theory]
        [InlineData("john.doe@example.com")]
        [InlineData("user@domain.org")]
        [InlineData("test.email@company.co")]
        public void CreateUser_ShouldSucceed_WithValidEmail(string email)
        {
            var result = UserFactory.CreateUser(email: email);

            Assert.False(result.IsError);
            Assert.Equal(email, result.Value.Email);
        }

        [Fact]
        public void CreateUser_ShouldFail_WhenRoleIsInvalid()
        {
            var result = UserFactory.CreateUser(role: (Role)999);

            Assert.True(result.IsError);
            Assert.Equal(UserErrors.RoleInvalid, result.Errors[0]);
        }

        [Theory]
        [InlineData(Role.Employee)]
        [InlineData(Role.Employer)]
        public void CreateUser_ShouldSucceed_WithAllValidRoles(Role role)
        {
            var result = UserFactory.CreateUser(role: role);

            Assert.False(result.IsError);
            Assert.Equal(role, result.Value.Role);
        }
    }
}
