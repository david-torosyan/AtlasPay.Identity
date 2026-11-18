
using System.Text.Json.Serialization;

namespace AtlasPay.Identity.Infrastructure.Keycloak;

internal sealed class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
