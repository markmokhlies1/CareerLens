using CareerLens.Application.Features.Identity;
using CareerLens.Application.Features.Identity.Dtos;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Identity;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.InterviewQuestions;
using CareerLens.Domain.Jobs;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Salaries;
using CareerLens.Domain.DomainUsers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Common.Interfaces
{

    #region DB
    public interface IAppDbContext
    {
        public DbSet<Company> Companies { get;}
        public DbSet<CompanyMember> CompanyMembers { get;}
        public DbSet<CompanyClaimRequest> CompanyClaimsRequests { get;}
        public DbSet<Interview> Interviews { get;} 
        public DbSet<InterviewQuestion> InterviewsQuestions { get;}
        public DbSet<Review> Reviews { get;}
        public DbSet<Job> Jobs { get;}
        public DbSet<Salary> Salaries { get;}
        public DbSet<RefreshToken> RefreshTokens { get; }
        public DbSet<User> DomainUsers { get;}
        public DbSet<Notification> Notifications { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
    #endregion

    #region Identity
    public interface IUser
    {
        string? Id { get; }
    }
    public interface IIdentityService
    {
        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string? policyName);

        Task<Result<AppUserDto>> AuthenticateAsync(string email, string password);

        Task<Result<AppUserDto>> GetUserByIdAsync(string userId);

        Task<string?> GetUserNameAsync(string userId);
    }
    #endregion

    #region Notification

    public interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(Guid userId, NotificationDto notification);
    }

    #endregion

    #region Token 
    public interface ITokenProvider
    {
        Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
    #endregion

    #region CompanyAuzorize
    public interface IRequireCompanyAccess
    {
        Guid CompanyId { get; }
        CompanyMemberRole[] AllowedRoles { get; }
    }
    #endregion

}
