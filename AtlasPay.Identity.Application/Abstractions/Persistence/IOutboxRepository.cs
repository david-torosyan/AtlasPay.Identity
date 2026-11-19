using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Application.Abstractions.Persistence;

public interface IOutboxRepository
{
    void Add(OutboxMessage message);
}