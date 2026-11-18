
using AtlasPay.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasPay.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.KeycloakUserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.KeycloakUserId)
            .IsUnique();

        builder.HasIndex(x => x.CustomerId)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
