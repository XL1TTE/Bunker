using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.PersonalityPresets.GetAllPersonalityPresets;

public readonly record struct GetAllPersonalityPresets()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<PersonalityPreset> Presets) : Result;
    }

    public static Result.Success Success(IReadOnlyCollection<PersonalityPreset> presets) => new(presets);
}
