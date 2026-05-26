using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IHobbiesCardRepository
{
    /// <inheritdoc />
    async Task<HobbiesCard?> IRepository<HobbiesCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.HobbiesCard>().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<HobbiesCard>();

    /// <inheritdoc />
    void IRepository<HobbiesCard, Card.Id>.Add(HobbiesCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<HobbiesCard, Card.Id>.Delete(HobbiesCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<HobbiesCard, Card.Id>.Update(HobbiesCard aggregate) => Update((Card)aggregate);
}
