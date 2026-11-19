using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;

namespace AtlasPay.Identity.Application.Features.Users.Commands.SetUserEnabled;

public sealed class SetUserEnabledHandler(
    IUserReadRepository userReadRepository,
    IIdentityProvider identityProvider)
{
    public async Task<bool> HandleAsync(
        SetUserEnabledCommand command,
        CancellationToken ct)
    {
        var user = await userReadRepository.GetByCustomerIdAsync(
            command.CustomerId, ct);

        if (user is null)
            return false;

        await identityProvider.SetUserEnabledAsync(
            user.KeycloakUserId,
            command.Enabled,
            ct);

        return true;
    }
}