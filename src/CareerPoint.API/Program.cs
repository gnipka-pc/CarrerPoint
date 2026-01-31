using CareerPoint.Application.Mappings;
using CareerPoint.Application.Services;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minio;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CareerPoint.API;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers().AddJsonOptions(options => 
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddDbContext<CareerPointContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Conenction string is empty");

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        builder.Services.AddMinio(options =>
        {
            var section = builder.Configuration.GetSection("Minio");
            options
                .WithEndpoint(section["Endpoint"])
                .WithCredentials(section["AccessKey"], section["SecretKey"])
                .WithSSL(bool.Parse(section["UseSsl"] ?? throw new InvalidOperationException("Ssl параметр не задан")))
                .Build();
        });

        builder.Services.AddTransient<IEventAppService, EventAppService>();
        builder.Services.AddTransient<IUserAppService, UserAppService>();
        builder.Services.AddTransient<IAuthAppService, AuthAppService>();
        builder.Services.AddTransient<INotificationAppService, NotificationAppService>();
        builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<CareerPointProfile>());
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
        builder.Services.AddAuthorization();

        var app = builder.Build();


        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CareerPointContext>();
            dbContext.Database.Migrate();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
