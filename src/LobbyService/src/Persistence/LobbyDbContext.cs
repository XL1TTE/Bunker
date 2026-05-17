using LobbyService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace LobbyService.Persistence;

public class LobbyDbContext(DbContextOptions<LobbyDbContext> options) : DbContext(options)
{
    public DbSet<LobbyEntity> Lobbies { get; init; }
    public DbSet<LobbyParticipantEntity> Participants { get; init; }
    public DbSet<LobbyCardPackEntity> CardPacks { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LobbyDbContext).Assembly);
        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
