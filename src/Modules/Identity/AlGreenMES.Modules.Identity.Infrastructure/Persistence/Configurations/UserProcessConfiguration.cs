using AlGreenMES.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlGreenMES.Modules.Identity.Infrastructure.Persistence.Configurations;

public class UserProcessConfiguration : IEntityTypeConfiguration<UserProcess>
{
    public void Configure(EntityTypeBuilder<UserProcess> builder)
    {
        builder.ToTable("user_processes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.ProcessId })
            .IsUnique();

        builder.HasIndex(x => x.ProcessId);
    }
}
