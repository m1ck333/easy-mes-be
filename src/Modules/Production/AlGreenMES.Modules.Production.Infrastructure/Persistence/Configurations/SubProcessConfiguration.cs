using AlGreenMES.Modules.Production.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Production.Infrastructure.Persistence.Configurations;

public class SubProcessConfiguration : IEntityTypeConfiguration<SubProcess>
{
    public void Configure(EntityTypeBuilder<SubProcess> builder)
    {
        builder.ToTable("sub_processes");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sp => sp.SequenceOrder)
            .IsRequired();

        builder.Property(sp => sp.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(sp => sp.CreatedAt)
            .IsRequired();

        builder.HasIndex(sp => new { sp.ProcessId, sp.SequenceOrder })
            .IsUnique();
    }
}
