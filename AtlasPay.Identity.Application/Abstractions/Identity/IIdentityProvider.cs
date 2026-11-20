
using AtlasPay.Identity.Application.Models;

namespace AtlasPay.Identity.Application.Abstractions.Identity;

public interface IIdentityProvider
{
    Task<string> CreateUserAsync(
        Guid requestId,
        Guid customerId,
        string email,
        string firstName,
        string lastName,
        CancellationToken ct);

    Task<IdentityUser?> GetUserByRegistrationRequestIdAsync(
        Guid requestId,
        CancellationToken ct);

    Task SetUserEnabledAsync(
        string keycloakUserId,
        bool enabled,
        CancellationToken ct);

    Task SendAccountActivationAsync(
        string keycloakUserId,
        CancellationToken ct);

    Task RevokeUserSessionsAsync(
        string keycloakUserId,
        CancellationToken ct);
}
