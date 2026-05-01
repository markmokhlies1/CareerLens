using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Identity.Dtos;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.DomainUsers.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Infrastructure.Services
{
    public class AppUser : IdentityUser<Guid>;
    public class IdentityService(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory, IAuthorizationService authorizationService) : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService = authorizationService;

        public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Error.NotFound("User_Not_Found", $"User with email {(email)} not found");
            }

            if (!user.EmailConfirmed)
            {
                return Error.Conflict("Email_Not_Confirmed", $"email '{(email)}' not confirmed");
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return Error.Conflict("Invalid_Login_Attempt", "Email / Password are incorrect");
            }

            return new AppUserDto(user.Id, user.Email!, await _userManager.GetRolesAsync(user), await _userManager.GetClaimsAsync(user));
        }

        public async Task<bool> AuthorizeAsync(Guid userId, string? policyName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policyName!);

            return result.Succeeded;
        }

        public async Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new InvalidOperationException(nameof(userId));

            var roles = await _userManager.GetRolesAsync(user);

            var claims = await _userManager.GetClaimsAsync(user);

            return new AppUserDto(user.Id, user.Email!, roles, claims);
        }

        public async Task<string?> GetUserNameAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user?.UserName;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null && await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<Result<Guid>> CreateUserAsync(Guid id, string email, string password, Role role)
        {
            var appUser = new AppUser
            {
                Id = id,                    
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => Error.Validation(e.Code, e.Description))
                    .ToList();
                return errors;
            }

            await _userManager.AddToRoleAsync(appUser, role.ToString());

            return id;
        }
        
    }
}
