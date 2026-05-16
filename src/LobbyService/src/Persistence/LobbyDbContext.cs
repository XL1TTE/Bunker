using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace LobbyService.Persistence;

internal class LobbyDbContext(DbContextOptions<LobbyDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
