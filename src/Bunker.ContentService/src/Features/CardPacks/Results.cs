using Bunker.ContentService.Messages;

namespace Bunker.ContentService.Features.CardPacks;

public abstract record CardPackResult;

public record CardPackUpdatedSuccess(CardPackUpdated Updated) : CardPackResult;
public record CardPackDeletedSuccess : CardPackResult;
public record CardPackSuccess(Domain.CardPack Pack) : CardPackResult;
public record CardPacksSuccess(IEnumerable<Domain.CardPack> Packs) : CardPackResult;
public record CardPackNotFound : CardPackResult;

public static class CardPackResultFactory
{
    extension(CardPackResult)
    {
        public static CardPackResult UpdatedSuccess(CardPackUpdated updated) => new CardPackUpdatedSuccess(updated);
        public static CardPackResult DeletedSuccess() => new CardPackDeletedSuccess();
        public static CardPackResult Success(Domain.CardPack pack) => new CardPackSuccess(pack);
        public static CardPackResult Success(IEnumerable<Domain.CardPack> packs) => new CardPacksSuccess(packs);
        public static CardPackResult NotFound() => new CardPackNotFound();
    }
}
