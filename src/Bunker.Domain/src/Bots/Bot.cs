using static Bunker.Domain.Bots.PersonalityPreset;

namespace Bunker.Domain.Bots;

public readonly record struct PersonalityPreset(PersonalityPresetId PublicId, string Title, string Description)
{
    public readonly record struct PersonalityPresetId(Guid Value)
    {
        public static PersonalityPresetId New() => new(Guid.NewGuid());
        public static PersonalityPresetId Restore(Guid value) => new(value);
    }
}

/// <summary>
/// Represents a bot player in the lobby. Bots have a name and a personality preset that defines their behavior and playstyle.
/// </summary>
/// <param name="Name">Name given to bot by the lobby host.</param>
/// <param name="PersonalityPreset">The personality preset that defines the bot's behavior and playstyle.</param>
public readonly record struct Bot(string Name, PersonalityPreset PersonalityPreset);

public static class BotFactory
{
    extension (Bot)
    {
        public static Bot Create(string name, PersonalityPreset personalityPreset)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Bot name cannot be empty");
            if (string.IsNullOrWhiteSpace(personalityPreset.Description))
                throw new ArgumentException("Personality preset description cannot be null or empty");

            return new Bot(name, personalityPreset);
        }
    }
}
