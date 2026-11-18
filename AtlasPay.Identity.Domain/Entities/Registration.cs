
using AtlasPay.Identity.Domain.Enums;

namespace AtlasPay.Identity.Domain.Entities;

public sealed class Registration
{
    private Registration() { } // EF Core

    public Guid Id { get; private set; }
    public Guid RequestId { get; private set; }
    public Guid CustomerId { get; private set; }

    public RegistrationStatus Status { get; private set; }

    public string? KeycloakUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Registration(Guid requestId, Guid customerId)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException(
                "RequestId is required.");

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "CustomerId is required.");

        Id = Guid.NewGuid();
        RequestId = requestId;
        CustomerId = customerId;

        Status = RegistrationStatus.Pending;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Complete(string keycloakUserId)
    {
        if (string.IsNullOrWhiteSpace(keycloakUserId))
            throw new ArgumentException(
                "KeycloakUserId is required.");

        if (Status != RegistrationStatus.Provisioning)
            throw new InvalidOperationException(
                "Only provisioning registrations can be completed.");

        KeycloakUserId = keycloakUserId;
        Status = RegistrationStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }


    public void StartProvisioning()
    {
        if (Status != RegistrationStatus.Pending)
            throw new InvalidOperationException(
                "Only pending registrations can start provisioning.");

        Status = RegistrationStatus.Provisioning;
        UpdatedAt = DateTime.UtcNow;
    }


    public void Fail()
    {
        if (Status == RegistrationStatus.Completed)
            throw new InvalidOperationException(
                "Completed registration cannot be failed.");

        Status = RegistrationStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
