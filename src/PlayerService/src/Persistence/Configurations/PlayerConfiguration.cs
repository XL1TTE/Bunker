using Bunker.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PlayerService.Persistence.Configurations;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.Id)
               .HasConversion(id => id.Value, value => PlayerId.Restore(value));

        builder.ComplexProperty(e => e.Nickname, builder =>
        {
            builder.Property(name => name.Value)
                   .HasColumnName("Nickname");
        });

        builder.ComplexProperty(e => e.Stats, builder =>
        {
            builder.Property(s => s.TotalGames).HasColumnName("TotalGames");
            builder.Property(s => s.Wins).HasColumnName("Wins");
            builder.Property(s => s.Losses).HasColumnName("Losses");
        });
    }
}
