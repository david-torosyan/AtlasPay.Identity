using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Infrastructure.Keycloak;
using AtlasPay.Identity.Infrastructure.Persistence;
using AtlasPay.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AtlasPay.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDb")));

        services.Configure<KeycloakOptions>(
            configuration.GetSection("Keycloak"));

        services.AddHttpClient<IIdentityProvider, KeycloakIdentityProvider>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        return services;
    }
}
