namespace Bunker.LobbyService.Persistence.Abstractions;

public interface IRepository;

public interface IRepository<TAggregate, TKey> : IRepository
{
    Task<TAggregate?> TryFindAsync(TKey key);
    void Add(TAggregate aggregate);
}