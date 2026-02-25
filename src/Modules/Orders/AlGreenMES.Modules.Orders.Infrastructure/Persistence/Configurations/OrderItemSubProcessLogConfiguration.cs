using AlGreenMES.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class OrderItemSubProcessLogConfiguration : IEntityTypeConfiguration<OrderItemSubProcessLog>
{
    public void Configure(EntityTypeBuilder<OrderItemSubProcessLog> builder)
    {
        builder.ToTable("order_item_sub_process_logs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.StartTime)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.HasIndex(l => l.OrderItemSubProcessId);
        builder.HasIndex(l => l.UserId);
    }
}
