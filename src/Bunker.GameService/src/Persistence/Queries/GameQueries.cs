using Bunker.GameService.Persistence.Contracts.Queries;
using Bunker.GameService.Sagas;
using Bunker.GameService.Transfers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.GameService.Persistence.Queries;

public sealed class GameQueries(GameDbContext db) : IGameQueries
{
    public Task<GameSaga?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
        => db.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == gameId, cancellationToken);

    public async Task<IReadOnlyList<ChatMessageDto>> GetChatMessagesAsync(Guid gameId, CancellationToken cancellationToken = default)
        => await db.GameChatMessages
            .AsNoTracking()
            .Where(x => x.GameId == gameId)
            .OrderBy(x => x.SentAt)
            .Select(x => new ChatMessageDto(
                Id: x.Id.ToString(),
                ParticipantId: x.ParticipantId,
                Nickname: x.Nickname,
                Text: x.Text,
                SentAt: x.SentAt))
            .ToListAsync(cancellationToken);
}