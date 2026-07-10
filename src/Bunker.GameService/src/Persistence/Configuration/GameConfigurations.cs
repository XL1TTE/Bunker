using Bunker.GameService.Persistence.Entities;
using Bunker.GameService.Persistence.Values;
using Bunker.GameService.Sagas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bunker.GameService.Persistence.Configurations;

public readonly record struct PersistenceConfigurations;

internal class GameSagaStateConfiguration : IEntityTypeConfiguration<GameSaga>
{
    public void Configure(EntityTypeBuilder<GameSaga> builder)
    {
        builder.ToTable("Games", "game");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartRequestId).IsRequired();
        builder.Property(x => x.LobbyId).IsRequired();
        builder.Property(x => x.HostId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.BunkerCapacity).IsRequired();
        builder.Property(x => x.Phase).HasMaxLength(32).IsRequired();
        builder.Property(x => x.RoundNumber).IsRequired();
        builder.Property(x => x.CurrentTurnIndex).IsRequired();
        builder.Property(x => x.TurnEpoch).IsRequired();
        builder.Property(x => x.VoteRound).IsRequired();

        // Primitive collections (EF Core 9+ maps List<string> to a jsonb column on Npgsql).
        builder.Property(x => x.TurnOrder);
        builder.Property(x => x.TiedParticipantIds);

        builder.OwnsOne(x => x.BunkerCard, b =>
        {
            b.ToJson("BunkerCard");
        });

        builder.OwnsMany(x => x.Participants, p =>
        {
            p.ToJson("Participants");
            p.OwnsMany(a => a.Attributes);
        });

        builder.OwnsMany(x => x.Votes, v =>
        {
            v.ToJson("Votes");
        });
    }
}

internal class GameChatMessageConfiguration : IEntityTypeConfiguration<GameChatMessage>
{
    public void Configure(EntityTypeBuilder<GameChatMessage> builder)
    {
        builder.ToTable("GameChatMessages", "game");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GameId).IsRequired();
        builder.Property(x => x.ParticipantId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Nickname).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Text).HasMaxLength(500).IsRequired();
        builder.Property(x => x.SentAt).IsRequired();

        builder.HasIndex(x => new { x.GameId, x.SentAt });
    }
}
