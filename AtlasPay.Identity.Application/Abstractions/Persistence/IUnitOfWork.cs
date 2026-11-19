
namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
