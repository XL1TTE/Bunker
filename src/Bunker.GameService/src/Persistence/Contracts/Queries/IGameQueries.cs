using Bunker.GameService.Sagas;
using Bunker.GameService.Transfers;

namespace Bunker.GameService.Persistence.Contracts.Queries;

public interface IGameQueries
{
    Task<GameSaga?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessageDto>> GetChatMessagesAsync(Guid gameId, CancellationToken cancellationToken = default);
}