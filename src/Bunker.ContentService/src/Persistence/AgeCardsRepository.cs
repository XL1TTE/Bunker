using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IAgeCardRepository
{
    /// <inheritdoc />
    async Task<AgeCard?> IRepository<AgeCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.AgeCard>().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<AgeCard>();

    /// <inheritdoc />
    void IRepository<AgeCard, Card.Id>.Add(AgeCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<AgeCard, Card.Id>.Delete(AgeCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<AgeCard, Card.Id>.Update(AgeCard aggregate) => Update((Card)aggregate);
}
