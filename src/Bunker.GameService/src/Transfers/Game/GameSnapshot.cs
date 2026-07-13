namespace Bunker.GameService.Transfers;

public record GameSnapshot(
    Guid Id,
    int RoundNumber,
    string Phase,
    int CurrentTurnIndex,
    int BunkerCapacity,
    BunkerCardDto BunkerCard,
    IReadOnlyList<ParticipantDto> Participants,
    IReadOnlyList<string> TurnOrder
);

public record BunkerCardDto(
    Guid Id,
    string Catastrophe,
    string SurvivalDuration,
    string BunkerEnvironment
);

public record ParticipantDto(
    string Id,
    string Nickname,
    string Type,
    bool Eliminated,
    bool IsYou,
    IReadOnlyList<AttributeDto> Attributes
);

public record AttributeDto(
    string Kind,
    string Value,
    bool Revealed
);
