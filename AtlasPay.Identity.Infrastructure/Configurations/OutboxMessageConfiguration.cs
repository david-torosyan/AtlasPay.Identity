using AtlasPay.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasPay.Identity.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration
    : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.LockedBy)
            .HasMaxLength(200);

        builder.Property(x => x.LastError)
            .HasColumnType("text");

        builder.HasIndex(x => new
        {
            x.ProcessedAt,
            x.NextAttemptAt,
            x.LockedUntil
        });
    }
}