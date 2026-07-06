using Bunker.LobbyService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence;

public class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options)
{
    
    public DbSet<AccountReadModel> Accounts => Set<AccountReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var accounts = modelBuilder.Entity<AccountReadModel>();
        
        accounts.Property<int>("Id").ValueGeneratedOnAdd();
        accounts.HasKey("Id");
        accounts.HasAlternateKey(x => x.PublicId);

        accounts.Property(x => x.PublicId).HasColumnName("AccountId")
               .HasConversion(id => id.Value, value => AccountId.Create(value));
    }
}
