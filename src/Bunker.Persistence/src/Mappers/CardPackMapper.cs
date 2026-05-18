using Bunker.Domain.Cards;
using Riok.Mapperly.Abstractions;

namespace Bunker.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(CardMapper))]
[UseStaticMapper(typeof(CardMapperExtensions))]
[UseStaticMapper(typeof(CardPackMapperExtensions))]
public static partial class CardPackMapper
{
    [MapperIgnoreTarget(nameof(CardPack.Cards))]
    [MapProperty(nameof(Entities.CardPack.Cards), nameof(CardPack._cards))]
    public static partial CardPack ToDomain(this Entities.CardPack cardPack);

    [MapperIgnoreSource(nameof(CardPack.Cards))]
    [MapProperty(nameof(CardPack._cards), nameof(Entities.CardPack.Cards))]
    public static partial Entities.CardPack ToEntity(this CardPack cardPack);

    [MapperIgnoreSource(nameof(CardPack.Cards))]
    [MapProperty(nameof(CardPack._cards), nameof(Entities.CardPack.Cards))]
    public static partial void ApplyUpdate([MappingTarget] this Entities.CardPack entity, CardPack cardPack);
}

internal static class CardPackMapperExtensions
{
    public static CardPack.Id MapId(this Guid cardPackId) => CardPack.Id.Create(cardPackId);
    public static Guid MapId(this CardPack.Id cardPackId) => cardPackId.Value;
}
