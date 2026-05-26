using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : ISexCardRepository
{
    /// <inheritdoc />
    async Task<SexCard?> IRepository<SexCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.SexCard>().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<SexCard>();

    /// <inheritdoc />
    void IRepository<SexCard, Card.Id>.Add(SexCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<SexCard, Card.Id>.Delete(SexCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<SexCard, Card.Id>.Update(SexCard aggregate) => Update((Card)aggregate);
}
