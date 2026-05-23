namespace Bunker.ContentService.Api.Cards.Endpoints.Responses;

public abstract record CardResponse
{  
    /// <summary>Response with Profession card.</summary>
    public readonly record struct ProfessionCard(Guid id, string Profession);

    /// <summary>Response with Hobbies card.</summary>
    public readonly record struct HobbiesCard(Guid id, string Hobbies);

    /// <summary>Response with Age card.</summary>
    public readonly record struct AgeCard(Guid id, int Age);

    /// <summary>Response with Sex card.</summary>
    public readonly record struct SexCard(Guid id, byte Sex);

    /// <summary>Response with Fact card.</summary>
    public readonly record struct FactCard(Guid id, string Fact);
}

