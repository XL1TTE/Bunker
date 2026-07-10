using Bunker.GameService.Persistence.Entities;

namespace Bunker.GameService.Persistence.Contracts;

/// <summary>
/// Tailored repository for game chat messages. Reached only via
/// <see cref="IUnitOfWork.GetRepository{TRepository}"/>; not registered in DI.
/// </summary>
public interface IGameChatMessageRepository : IRepository
{
    void Add(GameChatMessage message);

    Task<List<GameChatMessage>> ListByGameAsync(Guid gameId, CancellationToken cancellationToken = default);
}