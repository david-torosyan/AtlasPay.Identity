
using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IUserRepository
{
    void Add(User user);

    Task<User?> GetByKeycloakUserIdAsync(
        string keycloakUserId,
        CancellationToken ct);
}
