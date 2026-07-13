using Bunker.GameService.Messages;
using Bunker.GameService.Persistence.Contracts.Queries;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.GameService.Features.LeaveGame;

[WolverineHandler]
public static class LeaveGameHandler
{
    public static async Task<LeaveGame.Result> Handle(
        LeaveGame command,
        IGameQueries queries,
        IMessageContext messaging)
    {
        var game = await queries.GetGameAsync(command.GameId);
        if (game is null)
            return LeaveGame.Failure("Game not found.");

        var leaver = game.Participants.FirstOrDefault(p => p.AccountId == command.AccountId);
        if (leaver is null)
            return LeaveGame.Failure("You are not in this game.");

        if (game.Phase == "Finished")
            return LeaveGame.Failure("Game is already finished.");

        if (leaver.Eliminated)
            return LeaveGame.Failure("You have already been eliminated.");

        await messaging.PublishAsync(new LeaveRequested(
            GameId: command.GameId,
            ParticipantId: leaver.Id));

        return LeaveGame.Success();
    }
}