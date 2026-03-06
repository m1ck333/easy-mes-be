using AlGreenMES.Modules.Production.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("product_categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => new { c.TenantId, c.Name })
            .IsUnique();

        builder.HasMany(c => c.Processes)
            .WithOne(p => p.ProductCategory)
            .HasForeignKey(p => p.ProductCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Dependencies)
            .WithOne(d => d.ProductCategory)
            .HasForeignKey(d => d.ProductCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(ProductCategory.Processes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(ProductCategory.Dependencies))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
