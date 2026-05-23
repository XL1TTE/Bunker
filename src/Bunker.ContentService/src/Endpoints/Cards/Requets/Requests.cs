namespace Bunker.ContentService.Api.Cards.Endpoints.Requests;

public abstract record CardRequest
{
    public abstract record Post
    {
        /// <summary>Request to create a new Profession card.</summary>
        public readonly record struct ProfessionCard(string Profession);
        
        /// <summary>Request to create a new Hobbies card.</summary>
        public readonly record struct HobbiesCard(string Hobbies);

        /// <summary>Request to create a new Age card.</summary>
        public readonly record struct AgeCard(int Age);

        /// <summary>Request to create a new Sex card.</summary>
        public readonly record struct SexCard(string Sex);

        /// <summary>Request to create a new Fact card.</summary>
        public readonly record struct FactCard(string Fact);
    }
    
    public abstract record Put
    {
        /// <summary>Request to update an existing Profession card.</summary>
        public readonly record struct ProfessionCard(string Profession);

        /// <summary>Request to update an existing Hobbies card.</summary>
        public readonly record struct HobbiesCard(string Hobbies);

        /// <summary>Request to update an existing Age card.</summary>
        public readonly record struct AgeCard(int Age);

        /// <summary>Request to update an existing Sex card.</summary>
        public readonly record struct SexCard(string Sex);

        /// <summary>Request to update an existing Fact card.</summary>
        public readonly record struct FactCard(string Fact);
    }
}

