
namespace AtlasPay.Identity.Domain.Entities;

public class User
{
    private User() { } // EF Core

    public Guid Id { get; private set; }
    public string KeycloakUserId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User(string keycloakUserId, Guid customerId)
    {
        if (string.IsNullOrWhiteSpace(keycloakUserId))
            throw new ArgumentException(
                "Keycloak user ID is required.");

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer ID is required.");

        Id = Guid.NewGuid();
        KeycloakUserId = keycloakUserId;
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
    }
}
