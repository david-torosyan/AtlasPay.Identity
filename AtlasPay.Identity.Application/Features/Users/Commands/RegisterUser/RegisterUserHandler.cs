
using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Application.Features.Users.Commands;

public sealed class RegisterUserHandler(
    IIdentityProvider identityProvider,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Guid> HandleAsync(
        RegisterUserCommand command,
        CancellationToken ct)
    {
        var keycloakUserId = await identityProvider.CreateUserAsync(
            command.Email,
            command.FirstName,
            command.LastName,
            ct);

        var user = new User(
            keycloakUserId,
            command.CustomerId);

        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}
