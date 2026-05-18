using Bunker.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PlayerService.Persistence.Configurations;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "player");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(x => x.PublicId)
               .HasConversion(id => id.Value, value => User.Id.Restore(value));

        builder.HasAlternateKey(x => x.PublicId);

        builder.ComplexProperty(e => e.Nickname, builder =>
        {
            builder.Property(name => name.Value)
                   .HasColumnName("Nickname")
                   .HasMaxLength(32)
                   .IsRequired();
        });

        builder.ComplexProperty(e => e.Stats, builder =>
        {
            builder.Property(s => s.TotalGames).HasColumnName("TotalGames").IsRequired();
            builder.Property(s => s.Wins).HasColumnName("Wins").IsRequired();
            builder.Property(s => s.Losses).HasColumnName("Losses").IsRequired();
        });
    }
}
