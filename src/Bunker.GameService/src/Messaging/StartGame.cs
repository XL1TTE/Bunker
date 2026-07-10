namespace Bunker.GameService.Messages;

public record StartGame(
    Guid StartRequestId,
    Guid LobbyId,
    string HostId,
    IReadOnlyList<SagaParticipant> Participants,
    IReadOnlyList<GameProfessionCard> ProfessionCards,
    IReadOnlyList<GameHobbiesCard> HobbiesCards,
    IReadOnlyList<GameAgeCard> AgeCards,
    IReadOnlyList<GameSexCard> SexCards,
    IReadOnlyList<GameFactCard> FactCards,
    IReadOnlyList<GameHealthCard> HealthCards,
    IReadOnlyList<GameLuggageCard> LuggageCards,
    IReadOnlyList<GameBunkerCard> BunkerCards
);