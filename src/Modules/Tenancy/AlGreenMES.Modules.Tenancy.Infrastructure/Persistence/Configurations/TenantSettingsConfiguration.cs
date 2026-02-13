using AlGreenMES.Modules.Tenancy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Tenancy.Infrastructure.Persistence.Configurations;

public class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.ToTable("tenant_settings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.DefaultWarningDays)
            .IsRequired()
            .HasDefaultValue(7);

        builder.Property(s => s.DefaultCriticalDays)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(s => s.WarningColor)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("#FFA500");

        builder.Property(s => s.CriticalColor)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("#FF0000");

        builder.HasIndex(s => s.TenantId)
            .IsUnique();
    }
}
