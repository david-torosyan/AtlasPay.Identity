using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;

namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IUserReadRepository
{
    Task<UserResponse?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct);

    Task<UserResponse?> GetByKeycloakUserIdAsync(
        string keycloakUserId,
        CancellationToken ct);
}