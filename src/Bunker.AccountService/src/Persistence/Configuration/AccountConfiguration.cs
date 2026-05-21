using Bunker.AccountService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bunker.AccountService.Persistence;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Users", "player");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(x => x.PublicId)
               .HasConversion(id => id.Value, value => Account.Id.Create(value));

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
