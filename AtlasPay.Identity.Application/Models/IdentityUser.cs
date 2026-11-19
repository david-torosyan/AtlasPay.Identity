
namespace AtlasPay.Identity.Application.Models;

public sealed record IdentityUser(
    string Id,
    string Username,
    string Email,
    string FirstName,
    string LastName);
