using Bunker.ContentService.Persistence.Configurations;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext(DbContextOptions<ContentDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Card> Cards { get; init; }
    public DbSet<CardPack> CardPacks { get; init; }
    public DbSet<PersonalityPreset> PersonalityPresets { get; init; }

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
