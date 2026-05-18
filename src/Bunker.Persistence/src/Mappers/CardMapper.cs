
using Bunker.Domain.Cards;
using Riok.Mapperly.Abstractions;

namespace Bunker.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(CardMapperExtensions))]
public static partial class CardMapper
{
    [MapDerivedType<Entities.ProfessionCard, ProfessionCard>]
    [MapDerivedType<Entities.HobbiesCard, HobbiesCard>]
    [MapDerivedType<Entities.AgeCard, AgeCard>]
    [MapDerivedType<Entities.FactCard, FactCard>]
    [MapDerivedType<Entities.SexCard, SexCard>]
    public static partial Card ToDomain(this Entities.Card card);

    [MapDerivedType<ProfessionCard, Entities.ProfessionCard>]
    [MapDerivedType<HobbiesCard, Entities.HobbiesCard>]
    [MapDerivedType<AgeCard, Entities.AgeCard>]
    [MapDerivedType<FactCard, Entities.FactCard>]
    [MapDerivedType<SexCard, Entities.SexCard>]
    public static partial Entities.Card ToEntity(this Card card);

    public static partial void ApplyUpdate([MappingTarget] this Entities.Card entity, Card card);
}

internal static class CardMapperExtensions
{
    public static Card.Id MapId(this Guid cardId) => Card.Id.Create(cardId);
    public static Guid MapId(this Card.Id cardId) => cardId.Value;
}
