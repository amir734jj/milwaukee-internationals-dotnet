using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DAL.Entities;

public class StudentEntity : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasOne(x => x.Driver)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.DriverRefId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}