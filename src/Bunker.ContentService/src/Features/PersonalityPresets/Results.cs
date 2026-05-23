using Bunker.ContentService.Messages;

namespace Bunker.ContentService.Features.PersonalityPresets;

public abstract record PersonalityPresetResult;

public record PersonalityPresetUpdatedSuccess(PersonalityPresetUpdated Updated) : PersonalityPresetResult;
public record PersonalityPresetDeletedSuccess : PersonalityPresetResult;
public record PersonalityPresetSuccess(Domain.PersonalityPreset Preset) : PersonalityPresetResult;
public record PersonalityPresetsSuccess(IEnumerable<Domain.PersonalityPreset> Presets) : PersonalityPresetResult;
public record PersonalityPresetNotFound : PersonalityPresetResult;

public static class PersonalityPresetResultFactory
{
    extension(PersonalityPresetResult)
    {
        public static PersonalityPresetResult UpdatedSuccess(PersonalityPresetUpdated updated) => new PersonalityPresetUpdatedSuccess(updated);
        public static PersonalityPresetResult DeletedSuccess() => new PersonalityPresetDeletedSuccess();
        public static PersonalityPresetResult Success(Domain.PersonalityPreset preset) => new PersonalityPresetSuccess(preset);
        public static PersonalityPresetResult Success(IEnumerable<Domain.PersonalityPreset> presets) => new PersonalityPresetsSuccess(presets);
        public static PersonalityPresetResult NotFound() => new PersonalityPresetNotFound();
    }
}
