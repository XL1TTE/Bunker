using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.PersonalityPresets.CreatePersonalityPreset;

public readonly record struct CreatePersonalityPreset(string Title, string Description)
{
    public abstract record Result
    {
        public record Success(PersonalityPreset Preset) : Result;
    }

    public static Result.Success Success(PersonalityPreset preset) => new(preset);
}
