using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Configurations;

public class ChangeRequestConfiguration : IEntityTypeConfiguration<ChangeRequest>
{
    public void Configure(EntityTypeBuilder<ChangeRequest> builder)
    {
        builder.ToTable("change_requests");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.RequestType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(cr => cr.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(cr => cr.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(cr => cr.ResponseNote)
            .HasMaxLength(2000);

        builder.Property(cr => cr.CreatedAt)
            .IsRequired();

        builder.HasIndex(cr => cr.OrderId);
        builder.HasIndex(cr => new { cr.TenantId, cr.Status });

        builder.HasOne(cr => cr.Order)
            .WithMany()
            .HasForeignKey(cr => cr.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
