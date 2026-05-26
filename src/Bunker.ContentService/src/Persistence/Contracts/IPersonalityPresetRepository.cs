using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

public interface IPersonalityPresetRepository : IRepository<PersonalityPreset, PersonalityPreset.Id>;
