
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class RegistrationRepository(
    IdentityDbContext dbContext) : IRegistrationRepository
{
    public Task<Registration?> GetByRequestIdAsync(
        Guid requestId,
        CancellationToken ct)
        => dbContext.Registrations
            .FirstOrDefaultAsync(
                x => x.RequestId == requestId,
                ct);

    public void Add(Registration registration)
        => dbContext.Registrations.Add(registration);
}
