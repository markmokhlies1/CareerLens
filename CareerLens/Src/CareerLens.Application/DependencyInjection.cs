using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(RoleAuthorizationBehavior<,>));
                cfg.AddOpenBehavior(typeof(CompanyAuthorizationBehavior<,>));
                cfg.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
            });

            return services;
        }
    }
}
