using Bunker.GameService.Messages;
using Wolverine;

namespace Bunker.GameService.Sagas;

public class GameStartSaga : Saga
{
    public Guid Id { get; set; }
    public Guid LobbyId { get; set; }
    public string HostId { get; set; } = "";
    public List<Guid> CardPackIds { get; set; } = [];
    public List<Guid> PersonalityPresetIds { get; set; } = [];
    public List<SagaParticipant> Participants { get; set; } = [];
    public string Status { get; set; } = "AwaitingHydration";

    public static (GameStartSaga, RequestGameContentHydration) Start(GameStartRequested request)
    {
        var saga = new GameStartSaga
        {
            Id = request.StartRequestId,
            LobbyId = request.LobbyId,
            HostId = request.HostId,
            CardPackIds = request.CardPackIds.ToList(),
            PersonalityPresetIds = request.PersonalityPresetIds.ToList(),
            Participants = request.Participants.ToList(),
            Status = "AwaitingHydration"
        };

        return (saga, new RequestGameContentHydration(request.StartRequestId, request.CardPackIds, request.PersonalityPresetIds));
    }

    public object Handle(GameContentHydrated hydrated)
    {
        var missing = new List<string>();
        if (hydrated.ProfessionCards.Count == 0) missing.Add("Profession");
        if (hydrated.HobbiesCards.Count == 0) missing.Add("Hobbies");
        if (hydrated.AgeCards.Count == 0) missing.Add("Age");
        if (hydrated.SexCards.Count == 0) missing.Add("Sex");
        if (hydrated.FactCards.Count == 0) missing.Add("Fact");
        if (hydrated.HealthCards.Count == 0) missing.Add("Health");
        if (hydrated.LuggageCards.Count == 0) missing.Add("Luggage");
        if (hydrated.BunkerCards.Count == 0) missing.Add("Bunker");

        if (missing.Count > 0)
        {
            Status = "Failed";
            MarkCompleted();
            return new GameStartFailed(StartRequestId: Id, LobbyId, $"Insufficient canned content: missing {string.Join(", ", missing)} cards.");
        }

        Status = "Completed";
        MarkCompleted();

        return new StartGame(
            StartRequestId: Id,
            LobbyId,
            HostId,
            Participants,
            hydrated.ProfessionCards,
            hydrated.HobbiesCards,
            hydrated.AgeCards,
            hydrated.SexCards,
            hydrated.FactCards,
            hydrated.HealthCards,
            hydrated.LuggageCards,
            hydrated.BunkerCards);
    }

    public GameStartFailed Handle(GameContentHydrationFailed failed)
    {
        Status = "Failed";
        MarkCompleted();

        return new GameStartFailed(StartRequestId: Id, LobbyId, failed.Reason);
    }
}