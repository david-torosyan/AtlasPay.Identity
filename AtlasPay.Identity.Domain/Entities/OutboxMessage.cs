namespace AtlasPay.Identity.Domain.Entities;

public sealed class OutboxMessage
{
    private OutboxMessage() { } // For EF Core

    public Guid Id { get; private set; }
    public string Type { get; private set; } = null!;
    public string Payload { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public DateTime NextAttemptAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public int RetryCount { get; private set; }

    public string? LockedBy { get; private set; }
    public Guid? LockToken { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    public string? LastError { get; private set; }

    public OutboxMessage(string type, string payload)
    {
        Id = Guid.NewGuid();
        Type = type;
        Payload = payload;

        CreatedAt = DateTime.UtcNow;
        NextAttemptAt = CreatedAt;
    }

    public void Claim(
        string workerId,
        Guid lockToken,
        DateTime lockedUntil)
    {
        LockedBy = workerId;
        LockToken = lockToken;
        LockedUntil = lockedUntil;
    }
}