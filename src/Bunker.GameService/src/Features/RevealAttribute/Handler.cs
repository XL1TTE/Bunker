using Bunker.GameService.Messages;
using Bunker.GameService.Persistence.Contracts.Queries;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.GameService.Features.RevealAttribute;

[WolverineHandler]
public static class RevealAttributeHandler
{
    public static async Task<RevealAttribute.Result> Handle(
        RevealAttribute command,
        IGameQueries queries,
        IMessageContext messaging)
    {
        var game = await queries.GetGameAsync(command.GameId);
        if (game is null)
            return RevealAttribute.Failure("Game not found.");

        var viewer = game.Participants.FirstOrDefault(p => p.AccountId == command.AccountId);
        if (viewer is null)
            return RevealAttribute.Failure("You are not in this game.");

        if (game.Phase != "Reveal")
            return RevealAttribute.Failure("Reveal is only allowed in the Reveal phase.");

        if (game.CurrentTurnIndex < 0
            || game.CurrentTurnIndex >= game.TurnOrder.Count
            || game.TurnOrder[game.CurrentTurnIndex] != viewer.Id)
        {
            return RevealAttribute.Failure("It is not your turn.");
        }

        var slot = viewer.Attributes.FirstOrDefault(a => a.Kind == command.AttributeKind);
        if (slot is null)
            return RevealAttribute.Failure("Unknown attribute.");

        if (slot.Revealed)
            return RevealAttribute.Failure("Attribute is already revealed.");

        await messaging.PublishAsync(new AttributeRevealRequested(
            GameId: command.GameId,
            ParticipantId: viewer.Id,
            AttributeKind: command.AttributeKind));

        return RevealAttribute.Success();
    }
}