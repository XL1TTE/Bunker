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

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>()
        => (IRepository<TAggregate, TKey>)this;

    TRepository IUnitOfWork.GetRepository<TRepository>()
        => this as TRepository ?? throw new InvalidOperationException($"Repository {typeof(TRepository).Name} is not implemented.");
}