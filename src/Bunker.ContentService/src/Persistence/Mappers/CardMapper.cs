using Bunker.ContentService.Persistence.Entities;
using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(CardMapperExtensions))]
public static partial class CardMapper
{
    [MapDerivedType<ProfessionCard, Domain.ProfessionCard>]
    [MapDerivedType<HobbiesCard, Domain.HobbiesCard>]
    [MapDerivedType<AgeCard, Domain.AgeCard>]
    [MapDerivedType<FactCard, Domain.FactCard>]
    [MapDerivedType<SexCard, Domain.SexCard>]
    public static partial Domain.Card ToDomain(this Card card);

    [MapDerivedType<ProfessionCard, Domain.ProfessionCard>]
    [MapDerivedType<HobbiesCard, Domain.HobbiesCard>]
    [MapDerivedType<AgeCard, Domain.AgeCard>]
    [MapDerivedType<FactCard, Domain.FactCard>]
    [MapDerivedType<SexCard, Domain.SexCard>]
    public static partial T ToDomain<T>(this Card card) where T : Domain.Card;

    [MapDerivedType<Domain.ProfessionCard, ProfessionCard>]
    [MapDerivedType<Domain.HobbiesCard, HobbiesCard>]
    [MapDerivedType<Domain.AgeCard, AgeCard>]
    [MapDerivedType<Domain.FactCard, FactCard>]
    [MapDerivedType<Domain.SexCard, SexCard>]
    public static partial Card ToEntity(this Domain.Card card);

    [MapDerivedType<Domain.ProfessionCard, ProfessionCard>]
    [MapDerivedType<Domain.HobbiesCard, HobbiesCard>]
    [MapDerivedType<Domain.AgeCard, AgeCard>]
    [MapDerivedType<Domain.FactCard, FactCard>]
    [MapDerivedType<Domain.SexCard, SexCard>]
    public static partial void ApplyUpdate([MappingTarget] this Card entity, Domain.Card card);
}

internal static class CardMapperExtensions
{
    public static Domain.Card.Id MapId(this Guid cardId) => Domain.Card.Id.Create(cardId);
    public static Guid MapId(this Domain.Card.Id cardId) => cardId.Value;

}
