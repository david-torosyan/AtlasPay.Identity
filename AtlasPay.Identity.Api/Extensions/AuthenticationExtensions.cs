
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace AtlasPay.Identity.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority =
                    configuration["Keycloak:BaseUrl"]!
                    + "/realms/"
                    + configuration["Keycloak:Realm"];

                options.Audience = "atlaspay-identity-api";

                options.RequireHttpsMetadata = false; // as we use http for local development

                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("IdentityProvisioning", policy =>
            {
                policy.RequireAuthenticatedUser();

                policy.RequireClaim(
                    "azp",
                    "atlaspay-onboarding-service");

                policy.RequireAssertion(context =>
                {
                    var resourceAccess = context.User
                        .FindFirst("resource_access")?.Value;

                    if (string.IsNullOrWhiteSpace(resourceAccess))
                        return false;

                    try
                    {
                        using var document =
                            JsonDocument.Parse(resourceAccess);

                        return document.RootElement
                            .GetProperty("atlaspay-identity-api")
                            .GetProperty("roles")
                            .EnumerateArray()
                            .Any(role =>
                                role.GetString() == "provision-users");
                    }
                    catch (JsonException)
                    {
                        return false;
                    }
                    catch (KeyNotFoundException)
                    {
                        return false;
                    }
                });
            });
        });

        return services;
    }
}
