
using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IRegistrationRepository
{
    Task<Registration?> GetByRequestIdAsync(
        Guid requestId,
        CancellationToken ct);

    void Add(Registration registration);

    Task<bool> TryStartProvisioningAsync(
        Guid requestId,
        CancellationToken ct);

    Task<Registration> GetOrCreateAsync(
        Registration registration,
        CancellationToken ct);

    Task<Guid> CompleteAsync(
        Registration registration,
        string keycloakUserId,
        CancellationToken ct);
}
