using System.Text;
using CareGuard.Application.Common.Interfaces;
using CareGuard.Application.Common.Interfaces.Authentication;
using CareGuard.Domain.Models.Identity;
using CareGuard.Infrastructure.Data;
using CareGuard.Infrastructure.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CareGuard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDatabase(services);
        AddIdentity(services);
        AddAuthentication(services, configuration);
        AddServices(services);

        return services;
    }

    private static void AddDatabase(
        IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite("Data Source=app.db");
        });

        services.AddScoped<IAppDbContext, AppDbContext>();
    }

    private static void AddIdentity(
        IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    }

    private static void AddAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidIssuer =
                        configuration[
                            "JwtConfiguration:Issuer"],

                    ValidAudience =
                        configuration[
                            "JwtConfiguration:Audience"],

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                configuration[
                                    "JwtConfiguration:SecretKey"]!)),

                    ClockSkew = TimeSpan.Zero
                };
        });

        services.AddAuthorization();
    }

    private static void AddServices(
        IServiceCollection services)
    {
        services.AddScoped<
            IAuthenticationService,
            AuthenticationService>();
    }
}