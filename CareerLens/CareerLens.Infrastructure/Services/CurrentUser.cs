using CareerLens.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Infrastructure.Services
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
    {
        public string? Id => httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
