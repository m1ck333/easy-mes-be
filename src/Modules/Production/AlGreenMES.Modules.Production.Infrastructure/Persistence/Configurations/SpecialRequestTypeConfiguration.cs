using AlGreenMES.Modules.Production.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence.Configurations;

public class SpecialRequestTypeConfiguration : IEntityTypeConfiguration<SpecialRequestType>
{
    public void Configure(EntityTypeBuilder<SpecialRequestType> builder)
    {
        builder.ToTable("special_request_types");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.AddsProcesses)
            .HasColumnType("jsonb");

        builder.Property(s => s.RemovesProcesses)
            .HasColumnType("jsonb");

        builder.Property(s => s.OnlyProcesses)
            .HasColumnType("jsonb");

        builder.Property(s => s.IgnoresDependencies)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => new { s.TenantId, s.Code })
            .IsUnique();
    }
}
