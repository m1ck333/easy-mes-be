using AlGreenMES.Modules.Production.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence.Configurations;

public class ProcessConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.ToTable("processes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.SequenceOrder)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => new { p.TenantId, p.Code })
            .IsUnique();

        builder.HasMany(p => p.SubProcesses)
            .WithOne(sp => sp.Process)
            .HasForeignKey(sp => sp.ProcessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Process.SubProcesses))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
