using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;
using AtlasPay.Identity.Domain.Enums;
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


    public async Task<bool> TryStartProvisioningAsync(
        Guid requestId,
        CancellationToken ct)
    {
        var affectedRows = await dbContext.Registrations
            .Where(x =>
                x.RequestId == requestId &&
                x.Status == RegistrationStatus.Pending)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.Status,
                    RegistrationStatus.Provisioning)
                .SetProperty(
                    x => x.UpdatedAt,
                    DateTime.UtcNow),
                ct);

        return affectedRows == 1;
    }

    public async Task<Registration> GetOrCreateAsync(
        Registration registration,
        CancellationToken ct)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
        INSERT INTO "Registrations"
            ("Id", "RequestId", "CustomerId",
             "Email", "FirstName", "LastName",
             "Status", "CreatedAt", "UpdatedAt")
        VALUES
            ({registration.Id},
             {registration.RequestId},
             {registration.CustomerId},
             {registration.Email},
             {registration.FirstName},
             {registration.LastName},
             {registration.Status.ToString()},
             {registration.CreatedAt},
             {registration.UpdatedAt})
        ON CONFLICT ("RequestId") DO NOTHING
        """,
            ct);

        return await dbContext.Registrations
            .SingleAsync(x => x.RequestId == registration.RequestId, ct);
    }
}
