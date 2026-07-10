using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Messages;

public readonly record struct ProfessionCardUpdated(Transfer.ProfessionCard Card);
public readonly record struct HobbiesCardUpdated(Transfer.HobbiesCard Card);
public readonly record struct AgeCardUpdated(Transfer.AgeCard Card);
public readonly record struct SexCardUpdated(Transfer.SexCard Card);
public readonly record struct FactCardUpdated(Transfer.FactCard Card);
public readonly record struct HealthCardUpdated(Transfer.HealthCard Card);
public readonly record struct LuggageCardUpdated(Transfer.LuggageCard Card);

public readonly record struct CardDeleted(Guid Id);
