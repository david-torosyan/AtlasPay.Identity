
namespace AtlasPay.Identity.Api.Contracts.Users;

public sealed record RegisterUserRequest(
    Guid RequestId,
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName);
