using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.GetPersonalityPreset;

[WolverineHandler]
public static class GetPersonalityPresetHandler
{
    public static async Task<GetPersonalityPreset.Result> Handle(GetPersonalityPreset query, IPersonalityPresetQueries queries)
    {
        var domain = await queries.TryFindAsync(query.Id);
        return domain is null ? GetPersonalityPreset.NotFound() : GetPersonalityPreset.Success(domain);
    }
}
