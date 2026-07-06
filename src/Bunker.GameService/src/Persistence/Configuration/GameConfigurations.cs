using Bunker.GameService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bunker.GameService.Persistence.Configurations;

public readonly record struct PersistenceConfigurations;

internal class GameSessionConfiguration : IEntityTypeConfiguration<GameSessionEntity>
{
    public void Configure(EntityTypeBuilder<GameSessionEntity> builder)
    {
        builder.ToTable("GameSessions", "game");

        builder.HasKey(x => x.GameId);

        builder.Property(x => x.LobbyId).IsRequired();
        builder.Property(x => x.HostId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}