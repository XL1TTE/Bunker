using Bunker.GameService.Persistence.Contracts;
using Bunker.GameService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bunker.GameService.Persistence;

public partial class GameDbContext : IGameChatMessageRepository
{
    public void Add(GameChatMessage message) => GameChatMessages.Add(message);

    public Task<List<GameChatMessage>> ListByGameAsync(Guid gameId, CancellationToken cancellationToken = default)
        => GameChatMessages
            .AsNoTracking()
            .Where(x => x.GameId == gameId)
            .OrderBy(x => x.SentAt)
            .ToListAsync(cancellationToken);
}