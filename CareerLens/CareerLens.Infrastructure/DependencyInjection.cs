using CareerLens.Application.Common.Interfaces;
using CareerLens.Infrastructure.BackgroundJobs;
using CareerLens.Infrastructure.Data;
using CareerLens.Infrastructure.Data.Interceptors;
using CareerLens.Infrastructure.RealTime;
using CareerLens.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(TimeProvider.System);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            ArgumentNullException.ThrowIfNull(connectionString);

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                };
            });

            services
            .AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddSignalR();
            services.AddScoped<IRealTimeNotificationService, RealTimeNotificationService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUser, CurrentUser>();

            services.AddHostedService<PurgeClosedListingsJob>();
            services.AddHostedService<PurgeExpiredNotificationsJob>();
            services.AddHostedService<PurgeRejectedReviewsJob>();
            services.AddHostedService<PurgeRejectedSalariesJob>();
            services.AddHostedService<PurgeUnclaimedCompaniesJob>();


            return services;
        }
    }
}
