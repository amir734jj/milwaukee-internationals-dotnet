using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DAL.Entities;

public class DriverEntity : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasMany(x => x.Students)
            .WithOne(x => x.Driver)
            .HasForeignKey(x => x.DriverRefId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Host)
            .WithMany(x => x.Drivers)
            .HasForeignKey(x => x.HostRefId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.PushTokens)
            .WithOne(x => x.Driver)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}