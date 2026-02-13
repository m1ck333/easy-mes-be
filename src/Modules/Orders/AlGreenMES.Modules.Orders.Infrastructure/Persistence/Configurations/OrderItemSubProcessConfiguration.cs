using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class OrderItemSubProcessConfiguration : IEntityTypeConfiguration<OrderItemSubProcess>
{
    public void Configure(EntityTypeBuilder<OrderItemSubProcess> builder)
    {
        builder.ToTable("order_item_sub_processes");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sp => sp.TotalDurationMinutes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sp => sp.IsWithdrawn)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sp => sp.WithdrawnReason)
            .HasMaxLength(2000);

        builder.Property(sp => sp.StoppedReason)
            .HasMaxLength(2000);

        builder.HasIndex(sp => new { sp.OrderItemProcessId, sp.SubProcessId })
            .IsUnique();

        builder.HasMany(sp => sp.Logs)
            .WithOne(l => l.OrderItemSubProcess)
            .HasForeignKey(l => l.OrderItemSubProcessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(OrderItemSubProcess.Logs))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
