using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;

namespace AtlasPay.Identity.Application.Features.Users.Queries.GetUserByKeycloakId;

public sealed class GetUserByKeycloakIdHandler(
    IUserReadRepository userReadRepository)
{
    public Task<UserResponse?> HandleAsync(
        GetUserByKeycloakIdQuery query,
        CancellationToken ct)
        => userReadRepository.GetByKeycloakUserIdAsync(
            query.KeycloakUserId, ct);
}