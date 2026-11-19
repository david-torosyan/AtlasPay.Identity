
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(
    IdentityDbContext dbContext) : IUserRepository
{
    public void Add(User user)
        => dbContext.Users.Add(user);

    public Task<User?> GetByKeycloakUserIdAsync(
        string keycloakUserId,
        CancellationToken ct)
            => dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.KeycloakUserId == keycloakUserId,
                    ct);
}
