using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DAL.Entities;

public class HostEntity : IEntityTypeConfiguration<Host>
{
    public void Configure(EntityTypeBuilder<Host> builder)
    {
        builder.HasMany(x => x.Drivers)
            .WithOne(x => x.Host)
            .HasForeignKey(x => x.HostRefId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}