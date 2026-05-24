using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Entities;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IRepository<Domain.Card, Domain.Card.Id>
{
    public async Task<Domain.Card?> TryFindAsync(Domain.Card.Id key) => (await Cards.FindAsync(key.Value))?.ToDomain();
    
    public void Add(Domain.Card aggregate) => Cards.Add(aggregate.ToEntity());

    public void Delete(Domain.Card aggregate) => Cards.Remove(aggregate.ToEntity());

    public void Update(Domain.Card aggregate)
    {
        var origin = Cards.FirstOrDefault(x => Domain.Card.Id.Create(x.PublicId) == aggregate.PublicId);
        origin?.ApplyUpdate(aggregate);
    }
}
