using System.Security.Claims;
using System.Text.Json;

namespace AtlasPay.Identity.Api.Authorization;

public static class KeycloakRoleChecker
{
    public static bool HasClientRole(
        ClaimsPrincipal user,
        string clientId,
        string role)
    {
        var resourceAccess = user.FindFirst("resource_access")?.Value;

        if (string.IsNullOrWhiteSpace(resourceAccess))
            return false;

        try
        {
            using var document = JsonDocument.Parse(resourceAccess);

            if (!document.RootElement.TryGetProperty(
                    clientId, out var client))
                return false;

            if (!client.TryGetProperty("roles", out var roles) ||
                roles.ValueKind != JsonValueKind.Array)
                return false;

            return roles.EnumerateArray()
                .Any(value =>
                    value.ValueKind == JsonValueKind.String &&
                    value.GetString() == role);
        }
        catch (JsonException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}