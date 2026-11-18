
using AtlasPay.Identity.Application.Abstractions.Identity;
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

        request.Content = JsonContent.Create(new
        {
            username = email,
            email,
            firstName,
            lastName,
            enabled = true
        });

        using var response = await httpClient.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location
            ?? throw new InvalidOperationException(
                "Keycloak did not return the created user's location.");

        return location.Segments.Last().TrimEnd('/');
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
