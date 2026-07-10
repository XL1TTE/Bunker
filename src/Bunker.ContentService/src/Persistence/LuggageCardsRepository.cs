using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : ILuggageCardRepository
{
    /// <inheritdoc />
    async Task<LuggageCard?> IRepository<LuggageCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.LuggageCard>().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<LuggageCard>();

    /// <inheritdoc />
    void IRepository<LuggageCard, Card.Id>.Add(LuggageCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<LuggageCard, Card.Id>.Delete(LuggageCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<LuggageCard, Card.Id>.Update(LuggageCard aggregate) => Update((Card)aggregate);
}