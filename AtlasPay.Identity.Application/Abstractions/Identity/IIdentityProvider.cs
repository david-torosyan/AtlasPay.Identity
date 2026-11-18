
namespace AtlasPay.Identity.Application.Abstractions.Identity;

public interface IIdentityProvider
{
    Task<string> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        CancellationToken ct);
}
