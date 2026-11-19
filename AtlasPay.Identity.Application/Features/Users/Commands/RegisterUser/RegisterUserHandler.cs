
using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;
using AtlasPay.Identity.Domain.Enums;

namespace AtlasPay.Identity.Application.Features.Users.Commands;

public sealed class RegisterUserHandler(
    IIdentityProvider identityProvider,
    IUserRepository userRepository,
    IRegistrationRepository registrationRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Guid> HandleAsync(
        RegisterUserCommand command,
        CancellationToken ct)
    {
        var newRegistration = new Registration(
            command.RequestId,
            command.CustomerId,
            command.Email,
            command.FirstName,
            command.LastName);

        var registration = await registrationRepository
            .GetOrCreateAsync(newRegistration, ct);

        if (registration.CustomerId != newRegistration.CustomerId ||
            registration.Email != newRegistration.Email ||
            registration.FirstName != newRegistration.FirstName ||
            registration.LastName != newRegistration.LastName)
        {
            throw new InvalidOperationException(
                "RequestId is already used with different data.");
        }

        if (registration.Status == RegistrationStatus.Completed)
        {
            var existingUser = await userRepository
                .GetByKeycloakUserIdAsync(
                    registration.KeycloakUserId!, ct);

            return existingUser?.Id
                ?? throw new InvalidOperationException(
                    "Completed registration has no user mapping.");
        }

        if (registration.Status == RegistrationStatus.Provisioning)
            return await RecoverAsync(registration, ct);

        if (registration.Status != RegistrationStatus.Pending)
            throw new InvalidOperationException(
                "Registration cannot be processed.");

        var claimed = await registrationRepository
            .TryStartProvisioningAsync(command.RequestId, ct);

        if (!claimed)
            throw new InvalidOperationException(
                "Registration is already being processed.");

        registration.StartProvisioning();

        var keycloakUserId = await identityProvider.CreateUserAsync(
            registration.RequestId,
            registration.Email,
            registration.FirstName,
            registration.LastName,
            ct);

        var user = new User(
            keycloakUserId,
            registration.CustomerId);

        userRepository.Add(user);

        registration.Complete(keycloakUserId);

        await unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }

    private async Task<Guid> RecoverAsync(
        Registration registration,
        CancellationToken ct)
    {
        var keycloakUserId = await identityProvider
            .GetUserByRegistrationRequestIdAsync(
                registration.RequestId, ct);

        if (keycloakUserId is null)
            throw new InvalidOperationException(
                "Registration outcome is unresolved.");

        return await registrationRepository.CompleteAsync(
            registration,
            keycloakUserId,
            ct);
    }
}
