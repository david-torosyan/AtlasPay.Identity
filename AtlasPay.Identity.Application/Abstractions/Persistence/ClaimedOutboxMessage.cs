namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public sealed record ClaimedOutboxMessage(
    Guid Id,
    string Type,
    string Payload,
    Guid LockToken,
    int RetryCount);