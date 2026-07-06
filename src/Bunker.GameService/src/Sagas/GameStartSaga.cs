using Bunker.GameService.Messages;
using Bunker.GameService.Persistence.Contracts;
using Bunker.GameService.Persistence.Entities;
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
    public Guid? GameId { get; set; }

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

    public GameStartSucceeded Handle(GameContentHydrated hydrated, IUnitOfWork uow)
    {
        var gameId = Guid.NewGuid();

        var repository = uow.GetRepository<IGameSessionRepository>();
        repository.Add(new GameSessionEntity
        {
            GameId = gameId,
            LobbyId = LobbyId,
            HostId = HostId,
            Status = "Started",
            CreatedAt = DateTime.UtcNow
        });

        GameId = gameId;
        Status = "Completed";
        MarkCompleted();

        return new GameStartSucceeded(StartRequestId: Id, LobbyId, GameId: gameId, JoinUrl: $"/game/{gameId}");
    }

    public GameStartFailed Handle(GameContentHydrationFailed failed)
    {
        Status = "Failed";
        MarkCompleted();

        return new GameStartFailed(StartRequestId: Id, LobbyId, failed.Reason);
    }
}
