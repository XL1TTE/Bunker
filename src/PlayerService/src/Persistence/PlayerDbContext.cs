using Bunker.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace PlayerService.Persistence;

public class PlayerDbContext(DbContextOptions<PlayerDbContext> options) : DbContext(options)
{
    public DbSet<User> Players { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlayerDbContext).Assembly);
        
        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
