using AlGreenMES.Modules.Production.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence.Configurations;

public class ProductCategoryDependencyConfiguration : IEntityTypeConfiguration<ProductCategoryDependency>
{
    public void Configure(EntityTypeBuilder<ProductCategoryDependency> builder)
    {
        builder.ToTable("product_category_dependencies");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasOne(d => d.Process)
            .WithMany()
            .HasForeignKey(d => d.ProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.DependsOnProcess)
            .WithMany()
            .HasForeignKey(d => d.DependsOnProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.ProductCategoryId, d.ProcessId, d.DependsOnProcessId })
            .IsUnique();
    }
}
