using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.PersonalityPresets.UpdatePersonalityPreset;

public readonly record struct UpdatePersonalityPreset(PersonalityPreset.Id Id, string Title, string Description)
{
    public abstract record Result
    {
        public record Success(PersonalityPreset Preset) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(PersonalityPreset preset) => new(preset);
    public static Result.NotFound NotFound() => new();
}
