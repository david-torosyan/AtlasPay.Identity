using AtlasPay.Identity.Application.Abstractions;
using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;

namespace AtlasPay.Identity.Application.Features.Users.Commands.RevokeUserSessions;

public sealed class RevokeUserSessionsHandler(
    IUserReadRepository userReadRepository,
    IIdentityProvider identityProvider)
{
    public async Task<bool> HandleAsync(
        RevokeUserSessionsCommand command,
        CancellationToken ct)
    {
        var user = await userReadRepository.GetByCustomerIdAsync(
            command.CustomerId, ct);

        if (user is null)
            return false;

        await identityProvider.RevokeUserSessionsAsync(
            user.KeycloakUserId, ct);

        return true;
    }
}