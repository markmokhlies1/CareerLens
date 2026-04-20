using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.DomainUsers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.DomainUsers
{
    public sealed class User : AuditableEntity
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? Email { get; private set; }
        public Role Role { get; private set; }

        private User()
        {
        }
        private User(Guid id, string firstName, string lastName, string email, Role role) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
        }
        public static Result<User> Create(Guid id, string firstName, string lastName, string email, Role role)
        {

            if(id == Guid.Empty)
            {
                return UserErrors.IdRequired;
            }

            if (string.IsNullOrEmpty(firstName))
            {
                return UserErrors.FirstNameRequired;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                return UserErrors.LastNameRequired;
            }

            if (string.IsNullOrEmpty(email))
            {
                return UserErrors.EmailRequired;
            }

            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return UserErrors.EmailInvalid;
            }

            if (!Enum.IsDefined(role))
            {
                return UserErrors.RoleInvalid;
            }

            return new User(id, firstName, lastName, email, role);
        }
    }
}
