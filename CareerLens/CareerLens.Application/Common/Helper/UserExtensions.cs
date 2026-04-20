using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.DomainUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Common.Helper
{
    public static class UserExtensions
    {
        public static Result<Guid> GetUserId(this IUser user)
        {
            if (string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out var userId))
                return ApplicationErrors.Unauthenticated;

            return userId;
        }
    }
}
