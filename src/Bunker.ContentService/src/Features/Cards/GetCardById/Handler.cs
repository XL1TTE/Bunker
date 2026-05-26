using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.GetCardById;

[WolverineHandler]
public static class GetCardByIdHandler
{
    public static async Task<GetCardById.Result> Handle(GetCardById query, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardRepository>();
        var card = await repository.TryFindAsync(query.Id);

        if (card == null) return GetCardById.NotFound();

        return GetCardById.Success(card);
    }
}
