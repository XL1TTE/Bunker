
namespace Bunker.ContentService.Persistence.Contracts;

public interface IRepository;

public interface IRepository<TAggregate, TKey> : IRepository
{
    Task<TAggregate?> TryFindAsync(TKey key);
    bool Update(TAggregate aggregate);
    void Add(TAggregate aggregate);
    void Delete(TAggregate aggregate);
    
    async Task DeleteAsync(TKey key)
    {
        var record = await TryFindAsync(key) ?? throw new ArgumentException($"Unable to delete record by key: {key} because it is not found!");  
        Delete(record); 
    }
}
