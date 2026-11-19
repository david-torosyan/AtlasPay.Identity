using AtlasPay.Identity.Application.Abstractions.Identity;
using AtlasPay.Identity.Application.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AtlasPay.Identity.Infrastructure.Keycloak;

public sealed class KeycloakIdentityProvider(
    HttpClient httpClient,
    IOptions<KeycloakOptions> options) : IIdentityProvider
{
    private readonly KeycloakOptions _options = options.Value;

    public async Task<string> CreateUserAsync(
        Guid requestId,
        string email,
        string firstName,
        string lastName,
        CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);

        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users";

        using var request = new HttpRequestMessage(
            HttpMethod.Post, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var username = $"atlaspay-reg-{requestId:N}";

        request.Content = JsonContent.Create(new
        {
            username,
            email,
            firstName,
            lastName,
            enabled = true
        });

        using var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"Keycloak returned {(int)response.StatusCode}: {error}");
        }

        var location = response.Headers.Location
            ?? throw new InvalidOperationException(
                "Keycloak did not return the created user's location.");

        return location.Segments.Last().TrimEnd('/');
    }

    public async Task<IdentityUser?> GetUserByRegistrationRequestIdAsync(
        Guid requestId,
        CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);

        var username = $"atlaspay-reg-{requestId:N}";

        var url =
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users" +
            $"?username={Uri.EscapeDataString(username)}&exact=true";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();

        var users = await response.Content
            .ReadFromJsonAsync<List<KeycloakUserResponse>>(
                cancellationToken: ct);

        var user = users?.SingleOrDefault(
            x => string.Equals(
                x.Username,
                username,
                StringComparison.OrdinalIgnoreCase));

        return user is null
            ? null
            : new IdentityUser(
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName);
    }

    public async Task SetUserEnabledAsync(
        string keycloakUserId,
        bool enabled,
        CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);

        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{keycloakUserId}";

        using var request = new HttpRequestMessage(
            HttpMethod.Put, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = JsonContent.Create(new
        {
            enabled
        });

        using var response = await httpClient.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();
    }

    public async Task SendAccountActivationAsync(
        string keycloakUserId,
        CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);

        var url =
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}" +
            $"/users/{keycloakUserId}/execute-actions-email";

        using var request = new HttpRequestMessage(
            HttpMethod.Put, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = JsonContent.Create(new[]
        {
            "VERIFY_EMAIL",
            "UPDATE_PASSWORD"
        });

        using var response = await httpClient.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();
    }

    public async Task RevokeUserSessionsAsync(
    string keycloakUserId,
    CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);

        var url =
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}" +
            $"/users/{keycloakUserId}/logout";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();
    }

    private async Task<string> GetAccessTokenAsync(
        CancellationToken ct)
    {
        var tokenUrl = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            });

        using var response = await httpClient.PostAsync(
            tokenUrl,
            content,
            ct);

        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>(
            cancellationToken: ct);

        return token?.AccessToken
            ?? throw new InvalidOperationException(
                "Keycloak did not return an access token.");
    }
}
