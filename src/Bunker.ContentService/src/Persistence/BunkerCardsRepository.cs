using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Entities;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IBunkerCardRepository
{
    async Task<Domain.BunkerCard?> IRepository<Domain.BunkerCard, Domain.BunkerCard.Id>.TryFindAsync(Domain.BunkerCard.Id key)
        => (await BunkerCards.FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain();

    void IRepository<Domain.BunkerCard, Domain.BunkerCard.Id>.Add(Domain.BunkerCard aggregate)
        => BunkerCards.Add(aggregate.ToEntity());

    void IRepository<Domain.BunkerCard, Domain.BunkerCard.Id>.Delete(Domain.BunkerCard aggregate)
    {
        var entity = BunkerCards.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (entity != null)
        {
            BunkerCards.Remove(entity);
        }
    }

    bool IRepository<Domain.BunkerCard, Domain.BunkerCard.Id>.Update(Domain.BunkerCard aggregate)
    {
        var origin = BunkerCards.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (origin is null) return false;

        origin.ApplyUpdate(aggregate.ToEntity());
        return true;
    }
}