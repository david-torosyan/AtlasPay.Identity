
namespace AtlasPay.Identity.Infrastructure.Keycloak;

internal sealed class KeycloakUserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Attributes { get; init; }
}
