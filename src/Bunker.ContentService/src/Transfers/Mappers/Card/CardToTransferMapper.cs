using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Transfers;

[Mapper]
[UseStaticMapper(typeof(CardMapperExtensions))]
public static partial class CardToTransferMapper
{
    [MapDerivedType<Transfer.ProfessionCard, Domain.ProfessionCard>]
    [MapDerivedType<Transfer.HobbiesCard, Domain.HobbiesCard>]
    [MapDerivedType<Transfer.AgeCard, Domain.AgeCard>]
    [MapDerivedType<Transfer.FactCard, Domain.FactCard>]
    [MapDerivedType<Transfer.SexCard, Domain.SexCard>]
    [MapProperty(nameof(Transfer.Card.Id), nameof(Domain.Card.PublicId))]
    public static partial Domain.Card ToDomain(this Transfer.Card card);

    [MapDerivedType<Domain.ProfessionCard, Transfer.ProfessionCard>]
    [MapDerivedType<Domain.HobbiesCard, Transfer.HobbiesCard>]
    [MapDerivedType<Domain.AgeCard, Transfer.AgeCard>]
    [MapDerivedType<Domain.FactCard, Transfer.FactCard>]
    [MapDerivedType<Domain.SexCard, Transfer.SexCard>]
    [MapProperty(nameof(Domain.Card.PublicId), nameof(Transfer.Card.Id))]
    public static partial T ToTransferObject<T>(this Domain.Card card) where T: Transfer.Card;

    [MapDerivedType<Domain.ProfessionCard, Transfer.ProfessionCard>]
    [MapDerivedType<Domain.HobbiesCard, Transfer.HobbiesCard>]
    [MapDerivedType<Domain.AgeCard, Transfer.AgeCard>]
    [MapDerivedType<Domain.FactCard, Transfer.FactCard>]
    [MapDerivedType<Domain.SexCard, Transfer.SexCard>]
    [MapProperty(nameof(Domain.Card.PublicId), nameof(Transfer.Card.Id))]
    public static partial Transfer.Card ToTransferObject(this Domain.Card card);

    [MapProperty(nameof(Domain.ProfessionCard.PublicId), nameof(Transfer.ProfessionCard.Id))]
    public static partial Transfer.ProfessionCard ToTransferObject(this Domain.ProfessionCard card);

    [MapProperty(nameof(Domain.HobbiesCard.PublicId), nameof(Transfer.HobbiesCard.Id))]
    public static partial Transfer.HobbiesCard ToTransferObject(this Domain.HobbiesCard card);

    [MapProperty(nameof(Domain.SexCard.PublicId), nameof(Transfer.SexCard.Id))]
    public static partial Transfer.SexCard ToTransferObject(this Domain.SexCard card);

    [MapProperty(nameof(Domain.AgeCard.PublicId), nameof(Transfer.AgeCard.Id))]
    public static partial Transfer.AgeCard ToTransferObject(this Domain.AgeCard card);

    [MapProperty(nameof(Domain.FactCard.PublicId), nameof(Transfer.FactCard.Id))]
    public static partial Transfer.FactCard ToTransferObject(this Domain.FactCard card);
}

internal static class CardMapperExtensions
{
    public static Domain.Card.Id MapId(this Guid cardId) => Domain.Card.Id.Create(cardId);
    public static Guid MapId(this Domain.Card.Id cardId) => cardId.Value;
}
