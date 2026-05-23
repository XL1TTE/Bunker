using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence;
using Bunker.ContentService.Persistence.Entities;
using Bunker.ContentService.Messages;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks;

[WolverineHandler]
public static class CardPackHandlers
{
    public static async Task<CardPackResult> Handle(CreateCardPack command, ContentDbContext db)
    {
        var domain = CardPackFactory.New(command.Title, command.Description);
        foreach (var cardId in command.CardIds)
        {
            domain.AddCard(Domain.Card.Id.Create(cardId));
        }

        var entity = domain.ToEntity();
        db.CardPacks.Add(entity);
        await db.SaveChangesAsync();

        return CardPackResult.UpdatedSuccess(new CardPackUpdated(entity.PublicId, entity.Title, entity.Description, entity.Cards.Select(x => x.CardId)));
    }

    public static async Task<CardPackResult> Handle(UpdateCardPack command, ContentDbContext db)
    {
        var entity = await db.CardPacks
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(x => x.PublicId == command.Id);
            
        if (entity is null) return CardPackResult.NotFound();
        
        var update = new Domain.CardPack(new Domain.CardPack.Id(command.Id), command.Title, command.Description);
        
        foreach(var card in command.CardIds)
        {
            update.AddCard(Domain.Card.Id.Create(card));
        }

        entity.ApplyUpdate(update);
        
        await db.SaveChangesAsync();

        return CardPackResult.UpdatedSuccess(new CardPackUpdated(entity.PublicId, entity.Title, entity.Description, entity.Cards.Select(x => x.CardId)));
    }

    public static async Task<CardPackResult> Handle(DeleteCardPack command, ContentDbContext db)
    {
        var entity = await db.CardPacks.FirstOrDefaultAsync(x => x.PublicId == command.Id);
        if (entity is null) return CardPackResult.NotFound();

        db.CardPacks.Remove(entity);
        await db.SaveChangesAsync();
        
        return CardPackResult.DeletedSuccess();
    }

    public static async Task<CardPackResult> Handle(GetCardPack query, ContentDbContext db)
    {
        var entity = await db.CardPacks
            .Include(x => x.Cards)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == query.Id);
            
        return entity is null ? CardPackResult.NotFound() : CardPackResult.Success(entity.ToDomain());
    }

    public static async Task<CardPackResult> Handle(GetAllCardPacks query, ContentDbContext db)
    {
        var entities = await db.CardPacks
            .Include(x => x.Cards)
            .AsNoTracking()
            .ToListAsync();
            
        return CardPackResult.Success(entities.Select(x => x.ToDomain()));
    }

    public static async Task<CardPackResult> Handle(AddCardToPack command, ContentDbContext db)
    {
        var entity = await db.CardPacks
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(x => x.PublicId == command.CardPackId);
            
        if (entity is null) return CardPackResult.NotFound();

        var domain = entity.ToDomain();
        domain.AddCard(Domain.Card.Id.Create(command.CardId));

        entity.ApplyUpdate(domain);
        await db.SaveChangesAsync();

        return CardPackResult.UpdatedSuccess(new CardPackUpdated(entity.PublicId, entity.Title, entity.Description, entity.Cards.Select(x => x.CardId)));
    }

    public static async Task<CardPackResult> Handle(RemoveCardFromPack command, ContentDbContext db)
    {
        var entity = await db.CardPacks
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(x => x.PublicId == command.CardPackId);
            
        if (entity is null) return CardPackResult.NotFound();

        var domain = entity.ToDomain();
        domain.RemoveCard(Domain.Card.Id.Create(command.CardId));

        entity.ApplyUpdate(domain);
        await db.SaveChangesAsync();

        return CardPackResult.UpdatedSuccess(new CardPackUpdated(entity.PublicId, entity.Title, entity.Description, entity.Cards.Select(x => x.CardId)));
    }
}
