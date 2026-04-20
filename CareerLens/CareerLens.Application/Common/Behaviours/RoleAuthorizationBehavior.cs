using CareerLens.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Common.Behaviors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireRoleAttribute(string role) : Attribute
    {
        public string Role { get; } = role;
    }

    public sealed class RoleAuthorizationBehavior<TRequest, TResponse>(IUser user, IIdentityService identityService)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var roleAttributes = request.GetType()
                .GetCustomAttributes<RequireRoleAttribute>(true);

            if (!roleAttributes.Any())
                return await next();

            if (string.IsNullOrEmpty(user.Id))
                throw new UnauthorizedAccessException("User is not authenticated");

            foreach (var attr in roleAttributes)
            {
                if (await identityService.IsInRoleAsync(user.Id, attr.Role))
                    return await next();
            }

            throw new UnauthorizedAccessException(
                $"One of the following roles is required: " +
                $"{string.Join(", ", roleAttributes.Select(a => a.Role))}");
        }
    }
}
