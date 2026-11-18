
using AtlasPay.Identity.Application.Abstractions.Persistence;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(
    IdentityDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct)
        => dbContext.SaveChangesAsync(ct);
}