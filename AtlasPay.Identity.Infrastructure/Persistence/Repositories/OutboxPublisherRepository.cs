using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class OutboxPublisherRepository(
    IdentityDbContext dbContext)
    : IOutboxPublisherRepository
{
    public async Task<IReadOnlyList<ClaimedOutboxMessage>> ClaimBatchAsync(
        string workerId,
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerId);

        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize));

        if (leaseDuration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(leaseDuration));

        var messages = new List<ClaimedOutboxMessage>();

        var query = dbContext.Database
            .SqlQuery<ClaimedOutboxMessage>($"""
            WITH batch AS (
                SELECT "Id"
                FROM "OutboxMessages"
                WHERE "ProcessedAt" IS NULL
                  AND "NextAttemptAt" <= NOW()
                  AND (
                      "LockedUntil" IS NULL
                      OR "LockedUntil" <= NOW()
                  )
                ORDER BY "CreatedAt", "Id"
                LIMIT {batchSize}
                FOR UPDATE SKIP LOCKED
            )
            UPDATE "OutboxMessages" AS o
            SET "LockedBy" = {workerId},
                "LockToken" = gen_random_uuid(),
                "LockedUntil" = NOW() + {leaseDuration}
            FROM batch
            WHERE o."Id" = batch."Id"
            RETURNING
                o."Id",
                o."Type",
                o."Payload"::text AS "Payload",
                o."LockToken",
                o."RetryCount"
            """);

        await foreach (var message in query.AsAsyncEnumerable()
                                           .WithCancellation(ct))
        {
            messages.Add(message);
        }

        return messages;
    }

    public async Task<bool> MarkProcessedAsync(
        Guid messageId,
        Guid lockToken,
        CancellationToken ct)
    {
        var affected = await dbContext.Database
            .ExecuteSqlInterpolatedAsync($"""
                UPDATE "OutboxMessages"
                SET "ProcessedAt" = NOW(),
                    "LockedBy" = NULL,
                    "LockToken" = NULL,
                    "LockedUntil" = NULL,
                    "LastError" = NULL
                WHERE "Id" = {messageId}
                  AND "LockToken" = {lockToken}
                  AND "ProcessedAt" IS NULL
                """, ct);

        return affected == 1;
    }

    public async Task<bool> ScheduleRetryAsync(
        Guid messageId,
        Guid lockToken,
        DateTime nextAttemptAt,
        string error,
        CancellationToken ct)
    {
        var affected = await dbContext.Database
            .ExecuteSqlInterpolatedAsync($"""
                UPDATE "OutboxMessages"
                SET "RetryCount" = "RetryCount" + 1,
                    "NextAttemptAt" = {nextAttemptAt},
                    "LastError" = {error},
                    "LockedBy" = NULL,
                    "LockToken" = NULL,
                    "LockedUntil" = NULL
                WHERE "Id" = {messageId}
                  AND "LockToken" = {lockToken}
                  AND "ProcessedAt" IS NULL
                """, ct);

        return affected == 1;
    }
}