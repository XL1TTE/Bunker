
namespace Bunker.ContentService.Persistence.Contracts;

public interface IUnitOfWork
{
    IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>();
    TRepository GetRepository<TRepository>() where TRepository : class, IRepository;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
