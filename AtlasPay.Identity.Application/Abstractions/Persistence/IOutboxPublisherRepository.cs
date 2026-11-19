using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IOutboxPublisherRepository
{
    Task<IReadOnlyList<ClaimedOutboxMessage>> ClaimBatchAsync(
        string workerId,
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken ct);

    Task<bool> MarkProcessedAsync(
        Guid messageId,
        Guid lockToken,
        CancellationToken ct);

    Task<bool> ScheduleRetryAsync(
        Guid messageId,
        Guid lockToken,
        DateTime nextAttemptAt,
        string error,
        CancellationToken ct);
}