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
    public async Task<int> SaveChangesAsync() => await SaveChangesAsync();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
