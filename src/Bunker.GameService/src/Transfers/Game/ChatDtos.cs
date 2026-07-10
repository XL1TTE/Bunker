namespace Bunker.GameService.Transfers;

public record ChatMessageDto(
    string Id,
    string ParticipantId,
    string Nickname,
    string Text,
    DateTime SentAt
);

public record TallyDto(
    IReadOnlyList<VoteTallyEntry> Entries,
    int Abstains
);

public record VoteTallyEntry(
    string ParticipantId,
    int Count
);