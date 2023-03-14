using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DAL.Entities;

public class LocationMappingEntity : IEntityTypeConfiguration<LocationMapping>
{
    public void Configure(EntityTypeBuilder<LocationMapping> builder)
    {
        builder.HasOne(pt => pt.Source)
            .WithMany(p => p.LocationMappingsSources)
            .HasForeignKey(pt => pt.SourceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(pt => pt.Sink)
            .WithMany(p => p.LocationMappingsSinks)
            .HasForeignKey(pt => pt.SinkId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}