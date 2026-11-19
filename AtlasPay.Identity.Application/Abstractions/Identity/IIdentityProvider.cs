
namespace AtlasPay.Identity.Application.Abstractions.Identity;

public interface IIdentityProvider
{
    Task<string> CreateUserAsync(
        Guid requestId,
        string email,
        string firstName,
        string lastName,
        CancellationToken ct);

    Task<string?> GetUserByRegistrationRequestIdAsync(
        Guid requestId,
        CancellationToken ct);
}
