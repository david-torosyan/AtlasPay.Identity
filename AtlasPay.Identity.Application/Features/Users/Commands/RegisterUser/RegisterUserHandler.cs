using AtlasPay.Contracts.Events.Identity;
using AtlasPay.Contracts.Events.Identity.V1;
using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;
using AtlasPay.Identity.Domain.Enums;
using System.Text.Json;

namespace AtlasPay.Identity.Application.Features.Users.Commands;

public sealed class RegisterUserHandler(
    IIdentityProvider identityProvider,
    IUserRepository userRepository,
    IRegistrationRepository registrationRepository,
    IOutboxRepository outboxRepository,
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

        var keycloakUserId = await identityProvider.CreateUserAsync(
            registration.RequestId,
            registration.CustomerId,
            registration.Email,
            registration.FirstName,
            registration.LastName,
            ct);

        return await CompleteRegistrationAsync(
            registration.RequestId,
            keycloakUserId,
            ct);
    }

    private async Task<Guid> RecoverAsync(
        Registration registration,
        CancellationToken ct)
    {
        var identityUser = await identityProvider
            .GetUserByRegistrationRequestIdAsync(
                registration.RequestId, ct);

        if (identityUser is null)
            throw new InvalidOperationException(
                "Registration outcome is unresolved.");

        var expectedUsername =
            $"atlaspay-reg-{registration.RequestId:N}";

        if (!string.Equals(
                identityUser.Username,
                expectedUsername,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(
                identityUser.Email,
                registration.Email,
                StringComparison.OrdinalIgnoreCase) ||
            identityUser.FirstName != registration.FirstName ||
            identityUser.LastName != registration.LastName ||
            identityUser.CustomerId != registration.CustomerId ||
            string.IsNullOrWhiteSpace(identityUser.Id))
        {
            throw new InvalidOperationException(
                "Keycloak user does not match the original registration.");
        }

        return await CompleteRegistrationAsync(
            registration.RequestId,
            identityUser.Id,
            ct);
    }

    private async Task<Guid> CompleteRegistrationAsync(
        Guid requestId,
        string keycloakUserId,
        CancellationToken ct)
    {
        Guid userId = Guid.Empty;

        await unitOfWork.ExecuteInTransactionAsync(
            async token =>
            {
                var registration = await registrationRepository
                    .LockByRequestIdAsync(requestId, token);

                if (registration.Status == RegistrationStatus.Completed)
                {
                    if (registration.KeycloakUserId != keycloakUserId)
                        throw new InvalidOperationException(
                            "Registration has a different Keycloak user.");

                    var existingUser = await userRepository
                        .GetByKeycloakUserIdAsync(
                            keycloakUserId, token);

                    userId = existingUser?.Id
                        ?? throw new InvalidOperationException(
                            "Completed registration has no user mapping.");

                    return;
                }

                if (registration.Status != RegistrationStatus.Provisioning)
                    throw new InvalidOperationException(
                        "Registration is not ready for completion.");

                var user = new User(
                    keycloakUserId,
                    registration.CustomerId);

                userRepository.Add(user);

                registration.Complete(keycloakUserId);

                var eventId = Guid.NewGuid();

                var integrationEvent = new IdentityUserRegisteredV1(
                    eventId,
                    user.Id,
                    user.CustomerId,
                    registration.Email,
                    DateTime.UtcNow);

                var outboxMessage = new OutboxMessage(
                    IdentitiyEventTypes.IdentityUserRegisteredV1,
                    JsonSerializer.Serialize(
                        integrationEvent,
                        new JsonSerializerOptions(
                            JsonSerializerDefaults.Web)));

                outboxRepository.Add(outboxMessage);

                userId = user.Id;
            },
            ct);

        return userId;
    }
}