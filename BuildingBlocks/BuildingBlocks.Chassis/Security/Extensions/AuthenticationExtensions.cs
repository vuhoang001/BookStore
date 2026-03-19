using BuildingBlocks.Chassis.Security.Settings;
using BuildingBlocks.Chassis.Utils;
using BuildingBlocks.Chassis.Utils.Configuration;
using BuildingBlocks.Constants.Aspire;
using BuildingBlocks.Constants.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Chassis.Security.Extensions;

public static class AuthenticationExtensions
{
    public static void AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.Configure<IdentityOptions>(IdentityOptions.ConfigurationSection);

        var realm = services.BuildServiceProvider().GetRequiredService<IdentityOptions>().Realm;
        var scheme = builder.Environment.IsDevelopment()
            ? Http.Schemes.Http
            : Http.Schemes.HttpOrHttps;

        var keycloakUrl = HttpUtilities
            .AsUrlBuilder()
            .WithScheme(scheme)
            .WithHost(Components.KeyCloak)
            .Build();

        services.AddHttpClient(
            Components.KeyCloak,
            client => client.BaseAddress = new(keycloakUrl)
        );

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddKeycloakJwtBearer(
                Components.KeyCloak,
                realm,
                options =>
                {
                    
                    options.Authority            = $"{keycloakUrl}/realms/{realm}";
                    options.Audience             = "account";
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.TokenValidationParameters.ValidateAudience =
                        !builder.Environment.IsDevelopment();
                    options.TokenValidationParameters.ValidateIssuer =
                        !builder.Environment.IsDevelopment();
                }
            );
    }
}
