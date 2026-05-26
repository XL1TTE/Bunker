using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.PersonalityPresets.DeletePersonalityPreset;

public readonly record struct DeletePersonalityPreset(PersonalityPreset.Id Id)
{
    public abstract record Result
    {
        public record Success : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success() => new();
    public static Result.NotFound NotFound() => new();
}
