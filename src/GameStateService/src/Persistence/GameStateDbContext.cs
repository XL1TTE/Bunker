using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace GameStateService.Persistence;

internal class GameStateDbContext(DbContextOptions<GameStateDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Wolverine Outbox/Inbox tables
        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
