using AlGreenMES.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("push_subscriptions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Endpoint)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(p => p.P256dhKey)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(p => p.AuthKey)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => new { p.UserId, p.Endpoint })
            .HasFilter("is_active = true")
            .IsUnique();
    }
}
