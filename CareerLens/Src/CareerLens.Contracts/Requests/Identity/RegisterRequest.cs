using CareerLens.Domain.DomainUsers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Identity
{
    public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string ConfirmPassword, Role Role);
}
