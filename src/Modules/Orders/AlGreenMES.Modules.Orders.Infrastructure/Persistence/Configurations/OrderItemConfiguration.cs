using AlGreenMES.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.Notes)
            .HasMaxLength(2000);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasIndex(i => i.OrderId);

        builder.HasMany(i => i.Processes)
            .WithOne(p => p.OrderItem)
            .HasForeignKey(p => p.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.SpecialRequests)
            .WithOne(sr => sr.OrderItem)
            .HasForeignKey(sr => sr.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(OrderItem.Processes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(OrderItem.SpecialRequests))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
