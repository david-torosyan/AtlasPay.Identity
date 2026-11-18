namespace AtlasPay.Identity.Application.Features.Users.Commands;

public sealed record RegisterUserCommand(
    Guid RequestId,
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName);