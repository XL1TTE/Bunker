using Bunker.ContentService.Persistence.Configurations;
using Bunker.ContentService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public class ContentDbContext(DbContextOptions<ContentDbContext> options) : DbContext(options)
{
    public DbSet<Card> Cards { get; init; }
    public DbSet<CardPack> CardPacks { get; init; }
    public DbSet<PersonalityPreset> PersonalityPresets { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceConfigurations).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
