namespace Bunker.GameService.Messages;

public abstract partial record GameCard(Guid Id);

public record GameProfessionCard(Guid Id, string Profession) : GameCard(Id);

public record GameHobbiesCard(Guid Id, string Hobbies) : GameCard(Id);

public record GameAgeCard(Guid Id, int Age) : GameCard(Id);

public record GameSexCard(Guid Id, string Sex) : GameCard(Id);

public record GameFactCard(Guid Id, string Fact) : GameCard(Id);

public record GameHealthCard(Guid Id, string Health) : GameCard(Id);

public record GameLuggageCard(Guid Id, string Luggage) : GameCard(Id);

public record GameBunkerCard(Guid Id, string Catastrophe, string SurvivalDuration, string BunkerEnvironment);