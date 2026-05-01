using CareerLens.Domain.Common.Results;
using CareerLens.Domain.DomainUsers;
using CareerLens.Domain.DomainUsers.Enums;

namespace CareerLens.Tests.Common.Users
{
    public static class UserFactory
    {
        public static Result<User> CreateUser(
            Guid? id = null,
            string? firstName = null,
            string? lastName = null,
            string? email = null,
            Role? role = null)
        {
            return User.Create(
                id: id ?? Guid.NewGuid(),
                firstName: firstName ?? "John",
                lastName: lastName ?? "Doe",
                email: email ?? "john.doe@example.com",
                role: role ?? Role.Employee
            );
        }
    }
}
