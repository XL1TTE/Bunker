using Bunker.GameService.Persistence.Configurations;
using Bunker.GameService.Persistence.Contracts;
using Bunker.GameService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.GameService.Persistence;

public partial class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options), IUnitOfWork, IGameSessionRepository
{
    public DbSet<GameSessionEntity> GameSessions { get; init; } = null!;

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() => (IRepository<TAggregate, TKey>)this;

    public TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        => this as TRepository ?? throw new InvalidOperationException($"Repository {typeof(TRepository).Name} is not implemented.");

    public async Task<GameSessionEntity?> TryFindAsync(Guid key)
        => await GameSessions.AsNoTracking().FirstOrDefaultAsync(x => x.GameId == key);

    public bool Update(GameSessionEntity aggregate)
    {
        Entry(aggregate).State = EntityState.Modified;
        return true;
    }

    public void Add(GameSessionEntity session) => GameSessions.Add(session);

    public void Delete(GameSessionEntity aggregate) => GameSessions.Remove(aggregate);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage(databaseSchema: "wolverine");
    }
}