
using AtlasPay.Identity.Application.Abstractions.Persistence;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(
    IdentityDbContext dbContext) : IUnitOfWork
{
    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await operation(ct);

            await SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => dbContext.SaveChangesAsync(ct);
}