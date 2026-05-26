using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : ICardPackRepository
{
    async Task<Domain.CardPack?> IRepository<Domain.CardPack, Domain.CardPack.Id>.TryFindAsync(Domain.CardPack.Id key)
        => (await CardPacks.Include(x => x.Cards).FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain();

    void IRepository<Domain.CardPack, Domain.CardPack.Id>.Add(Domain.CardPack aggregate)
        => CardPacks.Add(aggregate.ToEntity());

    void IRepository<Domain.CardPack, Domain.CardPack.Id>.Delete(Domain.CardPack aggregate)
    {
        var entity = CardPacks.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (entity != null)
        {
            CardPacks.Remove(entity);
        }
    }

    bool IRepository<Domain.CardPack, Domain.CardPack.Id>.Update(Domain.CardPack aggregate)
    {
        var origin = CardPacks.Include(x => x.Cards).FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (origin is null) return false;

        origin.ApplyUpdate(aggregate);
        return true;
    }
}
