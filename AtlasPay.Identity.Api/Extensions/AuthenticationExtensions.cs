
using AtlasPay.Identity.Api.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
                    KeycloakRoleChecker.HasClientRole(
                        context.User,
                        "atlaspay-identity-api",
                        "provision-users"));
            });

            options.AddPolicy("IdentityRead", policy =>
            {
                policy.RequireAuthenticatedUser();

                policy.RequireClaim(
                    "azp",
                    "atlaspay-onboarding-service");

                policy.RequireAssertion(context =>
                    KeycloakRoleChecker.HasClientRole(
                        context.User,
                        "atlaspay-identity-api",
                        "read-users"));
            });

            options.AddPolicy("IdentityUserManagement", policy =>
            {
                policy.RequireAuthenticatedUser();

                policy.RequireClaim(
                    "azp",
                    "atlaspay-admin-service");

                policy.RequireAssertion(context =>
                    KeycloakRoleChecker.HasClientRole(
                        context.User,
                        "atlaspay-identity-api",
                        "manage-users"));
            });
        });

        return services;
    }
}
