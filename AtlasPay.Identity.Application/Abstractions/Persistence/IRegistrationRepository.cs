
using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IRegistrationRepository
{
    Task<Registration?> GetByRequestIdAsync(
        Guid requestId,
        CancellationToken ct);

    void Add(Registration registration);
}
