namespace AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;

public sealed record UserResponse(
    Guid Id,
    Guid CustomerId,
    string KeycloakUserId,
    DateTime CreatedAt);