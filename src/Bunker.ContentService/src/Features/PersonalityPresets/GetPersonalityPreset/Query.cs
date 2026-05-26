using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.PersonalityPresets.GetPersonalityPreset;

public readonly record struct GetPersonalityPreset(PersonalityPreset.Id Id)
{
    public abstract record Result
    {
        public record Success(PersonalityPreset Preset) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(PersonalityPreset preset) => new(preset);
    public static Result.NotFound NotFound() => new();
}
