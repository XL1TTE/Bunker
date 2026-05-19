using Bunker.LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bunker.LobbyService.Persistence;

public readonly record struct LobbyConfigurations;

internal class LobbyConfiguration : IEntityTypeConfiguration<Lobby>
{
    public void Configure(EntityTypeBuilder<Lobby> builder)
    {
        builder.ToTable("Lobbies", "lobby");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => x.PublicId);

        builder.Property(x => x.InviteCode).HasMaxLength(12).IsRequired();

        builder.HasIndex(x => x.InviteCode).IsUnique();

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.ComplexProperty(x => x.PrivacyPolicy);

        builder.HasMany(x => x.Participants)
            .WithOne()
            .HasPrincipalKey(x => x.PublicId)
            .HasForeignKey(x => x.LobbyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Packs)
            .WithOne()
            .HasForeignKey(x => x.LobbyId)
            .HasPrincipalKey(x => x.PublicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class LobbyParticipantConfiguration : IEntityTypeConfiguration<LobbyParticipant>
{
    public void Configure(EntityTypeBuilder<LobbyParticipant> builder)
    {
        builder.ToTable("Participants", "lobby");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => x.PublicId);

        builder.Property(x => x.Nickname)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(10)
            .IsRequired();

        builder.UseTptMappingStrategy();
    }
}

internal class PlayerParticipantConfiguration : IEntityTypeConfiguration<PlayerParticipant>
{
    public void Configure(EntityTypeBuilder<PlayerParticipant> builder)
    {
        builder.ToTable("Players", "lobby");

        builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
    }
}

internal class BotParticipantConfiguration : IEntityTypeConfiguration<BotParticipant>
{
    public void Configure(EntityTypeBuilder<BotParticipant> builder)
    {
        builder.ToTable("Bots", "lobby");

        builder.Property(x => x.PersonalityPresetId).HasColumnName("PersonalityId").IsRequired();
    }
}

internal class LobbyCardPackConfiguration : IEntityTypeConfiguration<LobbyCardPack>
{
    public void Configure(EntityTypeBuilder<LobbyCardPack> builder)
    {
        builder.ToTable("LobbyCardPacks", "lobby");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => new { x.LobbyId, x.PackId });
    }
}
