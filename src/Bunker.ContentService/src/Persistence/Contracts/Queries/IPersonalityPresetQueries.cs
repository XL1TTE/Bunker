using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

public interface IPersonalityPresetQueries
{
    Task<PersonalityPreset?> TryFindAsync(PersonalityPreset.Id id);
    Task<IReadOnlyCollection<PersonalityPreset>> GetAllAsync();
}
