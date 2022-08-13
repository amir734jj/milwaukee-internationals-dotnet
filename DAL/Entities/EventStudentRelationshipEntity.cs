using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DAL.Entities;

public class EventStudentRelationshipEntity : IEntityTypeConfiguration<EventStudentRelationship>
{
    public void Configure(EntityTypeBuilder<EventStudentRelationship> builder)
    {
        builder.HasKey(t => new { t.StudentId, t.EventId });

        builder.HasOne(pt => pt.Event)
            .WithMany(p => p.Students)
            .HasForeignKey(pt => pt.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(pt => pt.Event)
            .WithMany(t => t.Students)
            .HasForeignKey(pt => pt.EventId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}