using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DAL.Entities;

public class PushTokenEntity : IEntityTypeConfiguration<PushToken>
{
    public void Configure(EntityTypeBuilder<PushToken> builder)
    {
        // Driver can sign in with multiple accounts
        builder.HasKey(x => new
        {
            x.Token, x.DriverId
        });

        builder.HasOne(x => x.Driver)
            .WithMany(x => x.PushTokens)
            .HasForeignKey(x => x.DriverId);
    }
}