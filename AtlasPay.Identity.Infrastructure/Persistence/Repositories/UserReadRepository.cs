using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;
using Microsoft.EntityFrameworkCore;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserReadRepository(IdentityDbContext dbContext)
    : IUserReadRepository
{
    public Task<UserResponse?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct)
        => dbContext.Users
            .AsNoTracking()
            .Where(user => user.CustomerId == customerId)
            .Select(user => new UserResponse(
                user.Id,
                user.CustomerId,
                user.KeycloakUserId,
                user.CreatedAt))
            .FirstOrDefaultAsync(ct);

    public Task<UserResponse?> GetByKeycloakUserIdAsync(
        string keycloakUserId,
        CancellationToken ct)
        => dbContext.Users
            .AsNoTracking()
            .Where(user => user.KeycloakUserId == keycloakUserId)
            .Select(user => new UserResponse(
                user.Id,
                user.CustomerId,
                user.KeycloakUserId,
                user.CreatedAt))
            .FirstOrDefaultAsync(ct);
}