using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence.Queries;

public class DbContextCardQueries(ContentDbContext dbContext) : ICardQueries
{
    public async Task<(int, IReadOnlyCollection<Domain.AgeCard>)> GetAgeCardsAsync(int skip, int take)
    {
        var total = await dbContext.Cards.AsNoTracking()
            .OfType<Entities.AgeCard>().CountAsync();

        var cards = await dbContext.Cards.AsNoTracking().OfType<Entities.AgeCard>()
            .Skip(skip).Take(take)
            .Select(x => x.ToDomain<Domain.AgeCard>())
            .ToListAsync();
            
        return (total, cards);
    }

    public async Task<(int, IReadOnlyCollection<Domain.FactCard>)> GetFactCardsAsync(int skip, int take)
    {
        var total = await dbContext.Cards.AsNoTracking()
            .OfType<Entities.FactCard>().CountAsync();

        var cards = await dbContext.Cards.AsNoTracking().OfType<Entities.FactCard>()
            .Skip(skip).Take(take)
            .Select(x => x.ToDomain<Domain.FactCard>())
            .ToListAsync();

        return (total, cards);
    }

    public async Task<(int, IReadOnlyCollection<Domain.HobbiesCard>)> GetHobbiesCardsAsync(int skip, int take)
    {
        var total = await dbContext.Cards.AsNoTracking()
            .OfType<Entities.HobbiesCard>().CountAsync();

        var cards = await dbContext.Cards.AsNoTracking().OfType<Entities.HobbiesCard>()
            .Skip(skip).Take(take)
            .Select(x => x.ToDomain<Domain.HobbiesCard>())
            .ToListAsync();

        return (total, cards);
    }

    public async Task<(int, IReadOnlyCollection<Domain.ProfessionCard>)> GetProfessionCardsAsync(int skip, int take)
    {
        var total = await dbContext.Cards.AsNoTracking()
            .OfType<Entities.ProfessionCard>().CountAsync();
            
        var cards = await dbContext.Cards.AsNoTracking().OfType<Entities.ProfessionCard>()
            .Skip(skip).Take(take)
            .Select(x => x.ToDomain<Domain.ProfessionCard>())
            .ToListAsync();
            
        return (total, cards);
    }

    public async Task<(int, IReadOnlyCollection<Domain.SexCard>)> GetSexCardsAsync(int skip, int take)
    {
        var total = await dbContext.Cards.AsNoTracking()
            .OfType<Entities.SexCard>().CountAsync();

        var cards = await dbContext.Cards.AsNoTracking().OfType<Entities.SexCard>()
            .Skip(skip).Take(take)
            .Select(x => x.ToDomain<Domain.SexCard>())
            .ToListAsync();
            
        return (total, cards);
    }
}
