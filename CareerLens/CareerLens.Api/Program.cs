
using CareerLens.Application;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Infrastructure;
using CareerLens.Infrastructure.Services;
using CareerLens.Infrastructure.RealTime;

namespace CareerLens.Api
{
    public class Program
    {
        public  static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUser, CurrentUser>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();   
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");   

            

             app.RunAsync();

        }
    }
}
