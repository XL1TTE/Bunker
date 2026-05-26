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

    /// <summary>
    /// Response with paginated collection of age cards.
    /// </summary>
    /// <param name="Total">Number of cards in total.</param>
    /// <param name="Cards">Cards.</param>
    public readonly record struct AgeCards(int Total, IEnumerable<Transfer.AgeCard> Cards);

    /// <summary>
    /// Response with paginated collection of fact cards.
    /// </summary>
    /// <param name="Total">Number of cards in total.</param>
    /// <param name="Cards">Cards.</param>
    public readonly record struct FactCards(int Total, IEnumerable<Transfer.FactCard> Cards);

    /// <summary>
    /// Response with paginated collection of hobbies cards.
    /// </summary>
    /// <param name="Total">Number of cards in total.</param>
    /// <param name="Cards">Cards.</param>
    public readonly record struct HobbiesCards(int Total, IEnumerable<Transfer.HobbiesCard> Cards);

    /// <summary>
    /// Response with paginated collection of sex cards.
    /// </summary>
    /// <param name="Total">Number of cards in total.</param>
    /// <param name="Cards">Cards.</param>
    public readonly record struct SexCards(int Total, IEnumerable<Transfer.SexCard> Cards);
}
