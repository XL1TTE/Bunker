using LobbyService.Persistence.Entities;
using Bunker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;
using Bunker.Persistence.Configurations;
using LobbyService.Persistence.Configurations;

namespace LobbyService.Persistence;

public class LobbyDbContext(DbContextOptions<LobbyDbContext> options) : DbContext(options)
{
    public DbSet<Lobby> Lobbies { get; init; }
    public DbSet<LobbyParticipant> Participants { get; init; }
    public DbSet<LobbyCardPack> CardPacks { get; init; }

    public DbSet<PersonalityPreset> BotPersonalities { get; init; }
    public DbSet<CardPack> AvailableCardPacks { get; init; }
    public DbSet<Card> Cards { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LobbyConfigurations).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
