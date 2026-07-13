using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bunker.LobbyService.Persistence;

public readonly record struct LobbyConfigurations;

internal class LobbyConfiguration : IEntityTypeConfiguration<Domain.Lobby>
{
    public void Configure(EntityTypeBuilder<Domain.Lobby> builder)
    {
        builder.ToTable("Lobbies", "lobby");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => x.PublicId);

        builder.Property(x => x.PublicId)
            .HasConversion(ValueConversions.LobbyIdConverter, ValueConversions.LobbyIdComparer);

        builder.Property(x => x.InviteCode)
            .HasConversion(ValueConversions.InviteCodeConverter, ValueConversions.InviteCodeComparer)
            .HasMaxLength(12)
            .IsRequired();

        builder.HasIndex(x => x.InviteCode).IsUnique();

        builder.Property(x => x.Name)
            .HasConversion(ValueConversions.LobbyNameConverter, ValueConversions.LobbyNameComparer)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Capacity).IsRequired();

        builder.Property(x => x.State)
            .HasConversion(ValueConversions.LobbyStateConverter, ValueConversions.LobbyStateComparer)
            .HasColumnName("Status")
            .HasMaxLength(20).IsRequired();

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

        builder.Ignore(x => x.Players);
        builder.Ignore(x => x.Bots);
        builder.Ignore(x => x.Host);
    }
}

internal class LobbyParticipantConfiguration : IEntityTypeConfiguration<Domain.LobbyParticipant>
{
    public void Configure(EntityTypeBuilder<Domain.LobbyParticipant> builder)
    {
        builder.ToTable("Participants", "lobby");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => x.PublicId);

        builder.Property(x => x.PublicId)
            .HasConversion(ValueConversions.ParticipantIdConverter, ValueConversions.ParticipantIdComparer);

        builder.Property(x => x.LobbyId)
            .HasConversion(ValueConversions.LobbyIdConverter, ValueConversions.LobbyIdComparer);

        builder.Property(x => x.Nickname)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasConversion(ValueConversions.RoleConverter, ValueConversions.RoleComparer)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(ValueConversions.StatusConverter, ValueConversions.StatusComparer)
            .HasMaxLength(10)
            .IsRequired();

        builder.UseTptMappingStrategy();
    }
}

internal class PlayerParticipantConfiguration : IEntityTypeConfiguration<Domain.Player>
{
    public void Configure(EntityTypeBuilder<Domain.Player> builder)
    {
        builder.ToTable("Players", "lobby");

        builder.Property(x => x.UserId)
            .HasConversion(ValueConversions.AccountIdConverter, ValueConversions.AccountIdComparer)
            .HasColumnName("UserId")
            .IsRequired();
    }
}

internal class BotParticipantConfiguration : IEntityTypeConfiguration<Domain.BotParticipant>
{
    public void Configure(EntityTypeBuilder<Domain.BotParticipant> builder)
    {
        builder.ToTable("Bots", "lobby");

        builder.Property(x => x.PersonalityPresetId)
            .HasConversion(ValueConversions.BotPersonalityIdConverter, ValueConversions.BotPersonalityIdComparer)
            .HasColumnName("PersonalityId")
            .IsRequired();
    }
}

internal class LobbyCardPackConfiguration : IEntityTypeConfiguration<Domain.LobbyCardPack>
{
    public void Configure(EntityTypeBuilder<Domain.LobbyCardPack> builder)
    {
        builder.ToTable("LobbyCardPacks", "lobby");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(x => x.PackId)
            .HasConversion(ValueConversions.CardPackIdConverter, ValueConversions.CardPackIdComparer);

        builder.Property(x => x.LobbyId)
            .HasConversion(ValueConversions.LobbyIdConverter, ValueConversions.LobbyIdComparer);

        builder.HasAlternateKey(x => new { x.LobbyId, x.PackId });
    }
}
