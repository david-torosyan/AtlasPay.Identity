using System.Threading.RateLimiting;

namespace AtlasPay.Identity.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.AddPolicy("ActivationEmail", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Request.RouteValues["customerId"]?
                        .ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromMinutes(5) +
                                 TimeSpan.FromSeconds(Random.Shared.Next(0, 31)),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}