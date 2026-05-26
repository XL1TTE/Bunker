using Bunker.ContentService.Persistence.Entities;
using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(CardMapper))]
[UseStaticMapper(typeof(CardMapperExtensions))]
[UseStaticMapper(typeof(CardPackMapperExtensions))]
public static partial class CardPackMapper
{
    [MapperIgnoreTarget(nameof(CardPack.Cards))]
    [MapProperty(nameof(CardPack.Cards), nameof(Domain.CardPack._cards))]
    public static partial Domain.CardPack ToDomain(this CardPack cardPack);

    [MapperIgnoreSource(nameof(CardPack.Cards))]
    [MapProperty(nameof(Domain.CardPack._cards), nameof(CardPack.Cards))]
    public static partial CardPack ToEntity(this Domain.CardPack cardPack);

    [MapperIgnoreSource(nameof(Domain.CardPack.Cards))]
    [MapProperty(nameof(Domain.CardPack._cards), nameof(CardPack.Cards))]
    public static partial void ApplyUpdate([MappingTarget] this CardPack entity, Domain.CardPack cardPack);
}

internal static class CardPackMapperExtensions
{
    public static Domain.CardPack.Id MapId(this Guid cardPackId) => Domain.CardPack.Id.Create(cardPackId);
    public static Guid MapId(this Domain.CardPack.Id cardPackId) => cardPackId.Value;
}
