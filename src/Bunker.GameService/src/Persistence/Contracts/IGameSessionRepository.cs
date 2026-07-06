using Bunker.GameService.Persistence.Entities;

namespace Bunker.GameService.Persistence.Contracts;

public interface IGameSessionRepository : IRepository<GameSessionEntity, Guid>
{
}