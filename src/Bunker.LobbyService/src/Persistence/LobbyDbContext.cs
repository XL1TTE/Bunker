using Bunker.LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence;

public class LobbyDbContext(DbContextOptions<LobbyDbContext> options) : DbContext(options)
{
    public DbSet<Lobby> Lobbies { get; init; }
    public DbSet<LobbyParticipant> Participants { get; init; }
    public DbSet<LobbyCardPack> CardPacks { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LobbyConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
