using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IHealthCardRepository
{
    /// <inheritdoc />
    async Task<HealthCard?> IRepository<HealthCard, Card.Id>.TryFindAsync(Card.Id key)
        => (await Cards.OfType<Entities.HealthCard>().AsNoTracking().FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain<HealthCard>();

    /// <inheritdoc />
    void IRepository<HealthCard, Card.Id>.Add(HealthCard aggregate) => Add((Card)aggregate);

    /// <inheritdoc />
    void IRepository<HealthCard, Card.Id>.Delete(HealthCard aggregate) => Delete((Card)aggregate);

    /// <inheritdoc />
    bool IRepository<HealthCard, Card.Id>.Update(HealthCard aggregate) => Update((Card)aggregate);
}