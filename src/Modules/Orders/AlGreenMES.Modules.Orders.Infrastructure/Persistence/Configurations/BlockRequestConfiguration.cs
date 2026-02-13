using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class BlockRequestConfiguration : IEntityTypeConfiguration<BlockRequest>
{
    public void Configure(EntityTypeBuilder<BlockRequest> builder)
    {
        builder.ToTable("block_requests");

        builder.HasKey(br => br.Id);

        builder.Property(br => br.RequestNote)
            .HasMaxLength(2000);

        builder.Property(br => br.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(br => br.BlockReason)
            .HasMaxLength(2000);

        builder.Property(br => br.RejectionNote)
            .HasMaxLength(2000);

        builder.Property(br => br.CreatedAt)
            .IsRequired();

        builder.HasIndex(br => new { br.TenantId, br.Status });

        builder.HasOne(br => br.OrderItemProcess)
            .WithMany()
            .HasForeignKey(br => br.OrderItemProcessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(br => br.OrderItemSubProcess)
            .WithMany()
            .HasForeignKey(br => br.OrderItemSubProcessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
