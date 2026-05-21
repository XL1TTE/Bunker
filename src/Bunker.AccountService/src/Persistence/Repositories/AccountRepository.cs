
using Bunker.AccountService.Domain;
using Bunker.AccountService.Persistence.Repository;

namespace Bunker.AccountService.Persistence;

public partial class AccountDbContext : IAccountRepository, IUnitOfWork
{
    public bool Add(Account entity)
    {
        Accounts.Add(entity);
        return true;
    }

    public bool Delete(Account entity)
    {
        Accounts.Remove(entity);
        return true;
    }

    public Account? Find(Account.Id id) => Accounts.FirstOrDefault(x => x.PublicId == id);

    public void Rollback()
    {
       return;
    }

    public void Commit()
    {
        SaveChanges();
    }

    TRepository IUnitOfWork.GetRepository<TRepository>()
    {
        if(this is TRepository tRepository) return tRepository;
        throw new Exception($"DbContext not implementing {nameof(TRepository)}...");
    }

    public async Task CommitAsync()
    {
        await SaveChangesAsync();
    }

    public async Task RollbackAsync()
    {
        return;
    }
}
