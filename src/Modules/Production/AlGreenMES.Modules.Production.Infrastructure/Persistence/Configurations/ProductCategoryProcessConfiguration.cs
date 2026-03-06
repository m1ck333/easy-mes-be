using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence.Configurations;

public class ProductCategoryProcessConfiguration : IEntityTypeConfiguration<ProductCategoryProcess>
{
    public void Configure(EntityTypeBuilder<ProductCategoryProcess> builder)
    {
        builder.ToTable("product_category_processes");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.DefaultComplexity)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.SequenceOrder)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasOne(p => p.Process)
            .WithMany()
            .HasForeignKey(p => p.ProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.ProductCategoryId, p.ProcessId })
            .IsUnique();
    }
}
