using LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LobbyService.Persistence.Configurations;

internal class LobbyConfiguration : IEntityTypeConfiguration<LobbyEntity>
{
    public void Configure(EntityTypeBuilder<LobbyEntity> builder)
    {
        builder.ToTable("Lobbies");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.InviteCode).HasMaxLength(6).IsRequired();
        builder.HasIndex(x => x.InviteCode).IsUnique();

        builder.OwnsMany(x => x.Bots, b =>
        {
            b.ToJson();
        });

        builder.HasMany(x => x.Participants)
            .WithOne()
            .HasForeignKey(x => x.LobbyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Packs)
            .WithOne()
            .HasForeignKey(x => x.LobbyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class ParticipantConfiguration : IEntityTypeConfiguration<LobbyParticipantEntity>
{
    public void Configure(EntityTypeBuilder<LobbyParticipantEntity> builder)
    {
        builder.ToTable("LobbyParticipants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlayerId).IsRequired();
        builder.Property(x => x.Nickname).HasMaxLength(50).IsRequired();
    }
}

internal class CardPackConfiguration : IEntityTypeConfiguration<LobbyCardPackEntity>
{
    public void Configure(EntityTypeBuilder<LobbyCardPackEntity> builder)
    {
        builder.ToTable("LobbyCardPacks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PackId).IsRequired();
    }
}
