using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

public interface ICardPackQueries
{
    Task<CardPack?> TryFindAsync(CardPack.Id id);
    Task<IReadOnlyCollection<CardPack>> GetAllAsync();
}
