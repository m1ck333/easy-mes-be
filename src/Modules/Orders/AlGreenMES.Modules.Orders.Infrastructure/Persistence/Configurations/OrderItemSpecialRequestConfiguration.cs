using AlGreenMES.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class OrderItemSpecialRequestConfiguration : IEntityTypeConfiguration<OrderItemSpecialRequest>
{
    public void Configure(EntityTypeBuilder<OrderItemSpecialRequest> builder)
    {
        builder.ToTable("order_item_special_requests");

        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.CreatedAt)
            .IsRequired();

        builder.HasIndex(sr => new { sr.OrderItemId, sr.SpecialRequestTypeId })
            .IsUnique();
    }
}
