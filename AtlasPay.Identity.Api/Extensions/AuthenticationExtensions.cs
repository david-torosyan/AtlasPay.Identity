
using AtlasPay.Identity.Api.Authorization;
using AtlasPay.Security.Authentication;

namespace AtlasPay.Identity.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAtlasPayJwtAuthentication(configuration);

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
