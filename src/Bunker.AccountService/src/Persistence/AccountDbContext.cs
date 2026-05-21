using Bunker.AccountService.Domain;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Bunker.AccountService.Persistence;

public partial class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    internal DbSet<Account> Accounts { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);
        
        modelBuilder.MapWolverineEnvelopeStorage();
    }
}
