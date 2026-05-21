using Bunker.AccountService.Domain;

namespace Bunker.AccountService.Persistence.Repository;

public interface IAccountRepository : IRepository<Account, Account.Id>;

public interface IUnitOfWork
{
    TRepository GetRepository<TRepository>() where TRepository : IRepository;
    
    void Commit();
    Task CommitAsync();
    
    void Rollback();
    Task RollbackAsync();
}
