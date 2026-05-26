using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IFactCardRepository
{
    /// <inheritdoc />
    async Task<FactCard?> IRepository<FactCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.FactCard>().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<FactCard>();

    /// <inheritdoc />
    void IRepository<FactCard, Card.Id>.Add(FactCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<FactCard, Card.Id>.Delete(FactCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<FactCard, Card.Id>.Update(FactCard aggregate) => Update((Card)aggregate);
}
