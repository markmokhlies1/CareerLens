using CareerLens.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Common.Behaviours
{
    public sealed class CompanyAuthorizationBehavior<TRequest, TResponse>(IUser user, IIdentityService identityService, IAppDbContext context)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (request is not IRequireCompanyAccess companyRequest)
                return await next();

            if (string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated");

            if (await identityService.IsInRoleAsync(user.Id, "Admin"))
                return await next();

            var hasAccess = await context.CompanyMembers
                .AnyAsync(m => m.CompanyId == companyRequest.CompanyId
                            && m.UserId == userId
                            && companyRequest.AllowedRoles.Contains(m.Role), ct);

            if (!hasAccess)
                throw new UnauthorizedAccessException(
                    $"Requires one of: {string.Join(", ", companyRequest.AllowedRoles)}");

            return await next();
        }
    }
}
