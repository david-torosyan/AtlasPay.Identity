using AtlasPay.Identity.Application.Abstractions;
using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;

namespace AtlasPay.Identity.Application.Features.Users.Commands.SendAccountActivation;

public sealed class SendAccountActivationHandler(
    IUserReadRepository userReadRepository,
    IIdentityProvider identityProvider)
{
    public async Task<bool> HandleAsync(
        SendAccountActivationCommand command,
        CancellationToken ct)
    {
        var user = await userReadRepository.GetByCustomerIdAsync(
            command.CustomerId, ct);

        if (user is null)
            return false;

        await identityProvider.SendAccountActivationAsync(
            user.KeycloakUserId,
            ct);

        return true;
    }
}