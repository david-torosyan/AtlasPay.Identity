using AtlasPay.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtlasPay.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users
        => Set<User>();
    public DbSet<Registration> Registrations
        => Set<Registration>();
    public DbSet<OutboxMessage> OutboxMessages
        => Set<OutboxMessage>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IdentityDbContext).Assembly);
    }
}
