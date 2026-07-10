using Bunker.GameService.Persistence.Configurations;
using Bunker.GameService.Persistence.Contracts;
using Bunker.GameService.Persistence.Entities;
using Bunker.GameService.Sagas;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.GameService.Persistence;

public partial class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<GameSaga> Games { get; init; } = null!;
    public DbSet<GameChatMessage> GameChatMessages { get; init; } = null!;

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() => (IRepository<TAggregate, TKey>)this;

    public TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        => this as TRepository ?? throw new InvalidOperationException($"Repository {typeof(TRepository).Name} is not implemented.");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage(databaseSchema: "wolverine");
    }
}
