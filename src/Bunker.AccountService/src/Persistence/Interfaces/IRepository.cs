namespace Bunker.AccountService.Persistence.Repository;

public interface IRepository;

public interface IRepository<TEntity, TKey> : IRepository
{
    TEntity? Find(TKey id);
    bool Add(TEntity entity);
    bool Delete(TEntity entity);
}
