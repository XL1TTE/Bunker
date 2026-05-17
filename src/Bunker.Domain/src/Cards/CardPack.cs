using System.Collections.ObjectModel;
using static Bunker.Domain.Cards.Card;

namespace Bunker.Domain.Cards;

public class CardPack
{
    public readonly record struct CardPackId(Guid Value)
    {
        public static CardPackId Empty {get; } = new CardPackId(Guid.Empty);
        public static CardPackId New => new CardPackId(Guid.NewGuid());
        public static CardPackId Restore(Guid value) => new CardPackId(value);
    }

    public CardPackId PublicId { get; private set; }
    
    public Collection<CardId> CardsIds { get; private set; } = [];  
}
