using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.DomainUsers;
using CareerLens.Domain.Identity;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.InterviewQuestions;
using CareerLens.Domain.Jobs;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Salaries;
using CareerLens.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CareerLens.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options), IAppDbContext
    {
        public DbSet<Company> Companies => Set<Company>();

        public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();

        public DbSet<CompanyClaimRequest> CompanyClaimsRequests => Set<CompanyClaimRequest>();

        public DbSet<Interview> Interviews => Set<Interview>();

        public DbSet<InterviewQuestion> InterviewsQuestions => Set<InterviewQuestion>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<Job> Jobs => Set<Job>();

        public DbSet<Salary> Salaries => Set<Salary>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Notification> Notifications => Set<Notification>();

        public DbSet<User> DomainUsers => Set<User>();

        DatabaseFacade IAppDbContext.Database => Database;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEntities = ChangeTracker.Entries()
                .Where(e => e.Entity is Entity baseEntity && baseEntity.DomainEvents.Count != 0)
                .Select(e => (Entity)e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }

            foreach (var entity in domainEntities)
            {
                entity.ClearDomainEvents();
            }
        }
    }
}
