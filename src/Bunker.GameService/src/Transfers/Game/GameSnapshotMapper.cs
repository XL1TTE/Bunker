using Bunker.GameService.Sagas;

namespace Bunker.GameService.Transfers;

public static class GameSnapshotMapper
{
    public static GameSnapshot ToSnapshot(GameSaga game, string viewerAccountId)
    {
        var participants = game.Participants.Select(p =>
        {
            var isYou = p.AccountId == viewerAccountId;
            var attributes = isYou
                ? p.Attributes.Select(ToAttribute).ToList()
                : p.Attributes.Where(a => a.Revealed).Select(ToAttribute).ToList();

            return new ParticipantDto(p.Id, p.Nickname, p.Type, p.Eliminated, isYou, attributes);
        }).ToList();

        return new GameSnapshot(
            game.Id,
            game.RoundNumber,
            game.Phase,
            game.CurrentTurnIndex,
            game.BunkerCapacity,
            new BunkerCardDto(
                game.BunkerCard.Id,
                game.BunkerCard.Catastrophe,
                game.BunkerCard.SurvivalDuration,
                game.BunkerCard.BunkerEnvironment),
            participants,
            game.TurnOrder.ToList());
    }

    private static AttributeDto ToAttribute(Persistence.Values.AttributeSlotData attribute)
        => new(attribute.Kind, attribute.Value, attribute.Revealed);
}