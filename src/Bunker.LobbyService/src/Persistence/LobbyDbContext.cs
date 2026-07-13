using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence;

public partial class LobbyDbContext(DbContextOptions<LobbyDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Lobby> Lobbies { get; init; }
    public DbSet<LobbyParticipant> Participants { get; init; }
    public DbSet<LobbyCardPack> CardPacks { get; init; }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() => (IRepository<TAggregate, TKey>)this;

    public TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        => this as TRepository ?? throw new InvalidOperationException($"Repository {typeof(TRepository).Name} is not implemented.");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LobbyConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage("wolverine");
    }
}
