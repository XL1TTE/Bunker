namespace Bunker.GameService.Messages;

public record RequestGameContentHydration(
    Guid StartRequestId,
    IReadOnlyList<Guid> CardPackIds,
    IReadOnlyList<Guid> PersonalityPresetIds
);