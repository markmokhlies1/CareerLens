using CareerLens.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.DomainUsers
{
    public static class UserErrors
    {
        public static  Error IdRequired 
            => Error.Validation("User.Id.Required", "User Id is required.");
        public static Error FirstNameRequired 
            => Error.Validation("User.FirstName.Required", "First name is required.");

        public static Error LastNameRequired 
            => Error.Validation("User.LastName.Required", "Last name is required.");

        public static Error EmailRequired 
            => Error.Validation("User_Email_Required", "Email is required");

        public static Error EmailInvalid 
            => Error.Validation("User_Email_Invalid", "Email is invalid");

        public static Error UserExists 
            => Error.Conflict("User_Email_Exists", "A User with this email already exists.");

        public static Error RoleInvalid 
            => Error.Validation("User.Role.Invalid", "Invalid role assigned to User.");
        public static Error Forbidden
            => Error.Conflict("User.Forbidden", "User Role have not authorized to access");
    }
}
