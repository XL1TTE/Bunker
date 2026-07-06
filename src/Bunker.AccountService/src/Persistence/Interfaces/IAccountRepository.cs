using Bunker.AccountService.Domain;

namespace Bunker.AccountService.Persistence.Repository;

public interface IAccountRepository : IRepository<Account, Account.Id>;

public interface IUnitOfWork
{
    IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>();
    TRepository GetRepository<TRepository>() where TRepository : class, IRepository;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}