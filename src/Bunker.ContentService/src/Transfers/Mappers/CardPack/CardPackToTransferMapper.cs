using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Transfers;

[Mapper]
[UseStaticMapper(typeof(CardToTransferMapper))]
[UseStaticMapper(typeof(CardMapperExtensions))]
[UseStaticMapper(typeof(CardPackMapperExtensions))]
public static partial class CardPackMapper
{
    [MapProperty(nameof(Domain.CardPack.PublicId), nameof(Transfer.CardPack.Id))]
    [MapProperty(nameof(Domain.CardPack.Cards), nameof(Transfer.CardPack.CardIds))]
    [MapperIgnoreSource(nameof(Domain.CardPack._cards))]
    public static partial Transfer.CardPack ToTransferObject(this Domain.CardPack cardPack);
}

internal static class CardPackMapperExtensions
{
    public static Domain.CardPack.Id MapId(this Guid cardPackId) => Domain.CardPack.Id.Create(cardPackId);
    public static Guid MapId(this Domain.CardPack.Id cardPackId) => cardPackId.Value;

    public static IReadOnlyCollection<Guid> MapCards(IReadOnlyCollection<Domain.CardPackCards> cards)
          => cards.Select(x => x.CardId.Value).ToList();
}
