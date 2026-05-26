using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints.Responses;

/// <summary>
/// Container for card related HTTP response models.
/// </summary>
public abstract record CardResponse
{
    /// <summary>
    /// Response containing a profession card.
    /// </summary>
    /// <param name="Card">The profession card data.</param>
    public readonly record struct ProfessionCard(Transfer.ProfessionCard Card);

    /// <summary>
    /// Response containing a hobbies card.
    /// </summary>
    /// <param name="Card">The hobbies card data.</param>
    public readonly record struct HobbiesCard(Transfer.HobbiesCard Card);

    /// <summary>
    /// Response containing an age card.
    /// </summary>
    /// <param name="Card">The age card data.</param>
    public readonly record struct AgeCard(Transfer.AgeCard Card);

    /// <summary>
    /// Response containing a sex card.
    /// </summary>
    /// <param name="Card">The sex card data.</param>
    public readonly record struct SexCard(Transfer.SexCard Card);

    /// <summary>
    /// Response containing a fact card.
    /// </summary>
    /// <param name="Card">The fact card data.</param>
    public readonly record struct FactCard(Transfer.FactCard Card);

    /// <summary>
    /// Response containing any card.
    /// </summary>
    /// <param name="Card">The card data.</param>
    public readonly record struct Single(Transfer.Card Card);

    /// <summary>
    /// Response with paginated collection of profession cards.
    /// </summary>
    /// <param name="Total">Total number of profession cards available.</param>
    /// <param name="Cards">The collection of profession cards for the current page.</param>
    public readonly record struct ProfessionCards(int Total, IEnumerable<Transfer.ProfessionCard> Cards);

    /// <summary>
    /// Response with paginated collection of age cards.
    /// </summary>
    /// <param name="Total">Total number of age cards available.</param>
    /// <param name="Cards">The collection of age cards for the current page.</param>
    public readonly record struct AgeCards(int Total, IEnumerable<Transfer.AgeCard> Cards);

    /// <summary>
    /// Response with paginated collection of fact cards.
    /// </summary>
    /// <param name="Total">Total number of fact cards available.</param>
    /// <param name="Cards">The collection of fact cards for the current page.</param>
    public readonly record struct FactCards(int Total, IEnumerable<Transfer.FactCard> Cards);

    /// <summary>
    /// Response with paginated collection of hobbies cards.
    /// </summary>
    /// <param name="Total">Total number of hobbies cards available.</param>
    /// <param name="Cards">The collection of hobbies cards for the current page.</param>
    public readonly record struct HobbiesCards(int Total, IEnumerable<Transfer.HobbiesCard> Cards);

    /// <summary>
    /// Response with paginated collection of sex cards.
    /// </summary>
    /// <param name="Total">Total number of sex cards available.</param>
    /// <param name="Cards">The collection of sex cards for the current page.</param>
    public readonly record struct SexCards(int Total, IEnumerable<Transfer.SexCard> Cards);
}
