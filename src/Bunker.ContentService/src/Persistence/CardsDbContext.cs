using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : ICardRepository
{
    public async Task<Domain.Card?> TryFindAsync(Domain.Card.Id key)
        => (await Cards.AsNoTracking().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain();

    public void Add(Domain.Card aggregate) => Cards.Add(aggregate.ToEntity());

    public void Delete(Domain.Card aggregate)
    {
        var entity = Cards.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (entity is not null)
        {
            Cards.Remove(entity);
        }
    }

    public bool Update(Domain.Card aggregate)
    {
        var origin = Cards.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (origin is null) return false;

        origin?.ApplyUpdate(aggregate);
        return true;
    }
}
