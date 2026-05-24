using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints.Responses;

public abstract record CardResponse
{
    public readonly record struct ProfessionCard(Transfer.ProfessionCard Card);

    /// <summary>Response with Hobbies card.</summary>
    public readonly record struct HobbiesCard(Guid id, string Hobbies);

    /// <summary>Response with Age card.</summary>
    public readonly record struct AgeCard(Guid id, int Age);

    /// <summary>Response with Sex card.</summary>
    public readonly record struct SexCard(Guid id, byte Sex);

    /// <summary>Response with Fact card.</summary>
    public readonly record struct FactCard(Guid id, string Fact);

    /// <summary>
    /// Response with paginated collection of profession cards.
    /// </summary>
    /// <param name="Total">Number of cards in total.</param>
    /// <param name="Cards">Cards.</param>
    public readonly record struct ProfessionCards(int Total, IEnumerable<Transfer.ProfessionCard> Cards);
}
