using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class OutboxRepository(IdentityDbContext dbContext)
    : IOutboxRepository
{
    public void Add(OutboxMessage message)
        => dbContext.OutboxMessages.Add(message);
}