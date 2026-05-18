namespace Bunker.Domain.Bots;

/// <summary>
/// Represents a personality preset that defines the behavior and playstyle of a bot player in the lobby. Each personality preset has a unique public identifier, a title, and a description that provides details about the bot's characteristics and tendencies in gameplay.
/// </summary>
/// <param name="PublicId">The unique public identifier for the personality preset.</param>
/// <param name="Title">The title of the personality preset.</param>
/// <param name="Description">The description of the personality preset.</param>
public record PersonalityPreset(PersonalityPreset.Id PublicId, string Title, string Description)
{
    public readonly record struct Id(Guid Value)
    {
        public static Id New() => new(Guid.NewGuid());
        public static Id Create(Guid value) => new(value);
    }
    
    public string Title { get; internal set; } = 
        string.IsNullOrWhiteSpace(Title) | Title.Length < 4 
        ? throw new ArgumentException("Personality preset title must be at least 4 characters long") 
        : Title;
    
    public string Description { get; internal set; } = 
        string.IsNullOrWhiteSpace(Description) | Description.Length < 10 
        ? throw new ArgumentException("Personality preset description must be at least 10 characters long") 
        : Description;
}

public static class PersonalityPresetFactory
{
    extension(PersonalityPreset)
    {
        public static PersonalityPreset New(string title, string description) 
            => new PersonalityPreset(PersonalityPreset.Id.New(), title, description);
        public static PersonalityPreset Create(Guid id, string title, string description)
            => new PersonalityPreset(PersonalityPreset.Id.Create(id), title, description);
    }
}

public static class PersonalityPresetExtensions
{
    extension(PersonalityPreset preset)
    {
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < 4)
                throw new ArgumentException("Personality preset title must be at least 4 characters long");
         
            preset.Title = title;
        }
        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description) || description.Length < 10)
                throw new ArgumentException("Personality preset description must be at least 10 characters long");
         
            preset.Description = description;
        }
    }
}
