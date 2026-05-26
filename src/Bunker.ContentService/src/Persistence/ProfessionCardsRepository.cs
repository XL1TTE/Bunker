using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IProfessionCardRepository
{
    /// <inheritdoc />
    async Task<ProfessionCard?> IRepository<ProfessionCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.ProfessionCard>().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<ProfessionCard>();

    /// <inheritdoc />
    void IRepository<ProfessionCard, Card.Id>.Add(ProfessionCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<ProfessionCard, Card.Id>.Delete(ProfessionCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<ProfessionCard, Card.Id>.Update(ProfessionCard aggregate) => Update((Card)aggregate);
}
