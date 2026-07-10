using Bunker.GameService.Messages;
using Bunker.GameService.Persistence.Values;

namespace Bunker.GameService.GameConstruction;

public static class GameInitializer
{
    public static (List<ParticipantData> Participants, BunkerCardData BunkerCard, int BunkerCapacity) Initialize(
        IReadOnlyList<SagaParticipant> participants,
        IReadOnlyList<GameProfessionCard> professionCards,
        IReadOnlyList<GameHobbiesCard> hobbiesCards,
        IReadOnlyList<GameAgeCard> ageCards,
        IReadOnlyList<GameSexCard> sexCards,
        IReadOnlyList<GameFactCard> factCards,
        IReadOnlyList<GameHealthCard> healthCards,
        IReadOnlyList<GameLuggageCard> luggageCards,
        IReadOnlyList<GameBunkerCard> bunkerCards)
    {
        var rng = Random.Shared;

        var professionOrder = Shuffle(professionCards, rng);
        var hobbiesOrder = Shuffle(hobbiesCards, rng);
        var ageOrder = Shuffle(ageCards, rng);
        var sexOrder = Shuffle(sexCards, rng);
        var factOrder = Shuffle(factCards, rng);
        var healthOrder = Shuffle(healthCards, rng);
        var luggageOrder = Shuffle(luggageCards, rng);

        var built = new List<ParticipantData>(participants.Count);
        for (var i = 0; i < participants.Count; i++)
        {
            var p = participants[i];
            built.Add(new ParticipantData
            {
                Id = p.Id,
                Nickname = p.Nickname,
                Type = p.Type,
                PersonalityPresetId = p.PersonalityPresetId,
                AccountId = p.AccountId,
                Eliminated = false,
                Attributes =
                [
                    Slot("Profession", professionOrder[i % professionOrder.Count]),
                    Slot("Hobbies", hobbiesOrder[i % hobbiesOrder.Count]),
                    Slot("Age", ageOrder[i % ageOrder.Count]),
                    Slot("Sex", sexOrder[i % sexOrder.Count]),
                    Slot("Fact", factOrder[i % factOrder.Count]),
                    Slot("Health", healthOrder[i % healthOrder.Count]),
                    Slot("Luggage", luggageOrder[i % luggageOrder.Count]),
                ]
            });
        }

        var chosen = bunkerCards[rng.Next(bunkerCards.Count)];
        var bunkerCard = new BunkerCardData
        {
            Id = chosen.Id,
            Catastrophe = chosen.Catastrophe,
            SurvivalDuration = chosen.SurvivalDuration,
            BunkerEnvironment = chosen.BunkerEnvironment
        };

        return (built, bunkerCard, ComputeCapacity(participants.Count, rng));
    }

    private static List<T> Shuffle<T>(IReadOnlyList<T> source, Random rng)
    {
        var list = new List<T>(source);
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    private static AttributeSlotData Slot<TCard>(string kind, TCard card) where TCard : GameCard
    {
        var (cardId, value) = Extract(card);
        return new AttributeSlotData { Kind = kind, CardId = cardId, Value = value, Revealed = false };
    }

    private static (Guid Id, string Value) Extract(GameCard card) => card switch
    {
        GameProfessionCard c => (c.Id, c.Profession),
        GameHobbiesCard c => (c.Id, c.Hobbies),
        GameAgeCard c => (c.Id, c.Age.ToString()),
        GameSexCard c => (c.Id, c.Sex),
        GameFactCard c => (c.Id, c.Fact),
        GameHealthCard c => (c.Id, c.Health),
        GameLuggageCard c => (c.Id, c.Luggage),
        _ => throw new InvalidOperationException($"Unsupported card kind {card?.GetType().Name}.")
    };

    private static int ComputeCapacity(int participantCount, Random rng)
    {
        var percent = rng.Next(3) switch { 0 => 0.30, 1 => 0.40, _ => 0.50 };
        var capacity = (int)Math.Round(participantCount * percent);
        return capacity < 1 ? 1 : capacity;
    }
}