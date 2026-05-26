using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.GetAllPersonalityPresets;

[WolverineHandler]
public static class GetAllPersonalityPresetsHandler
{
    public static async Task<GetAllPersonalityPresets.Result> Handle(GetAllPersonalityPresets query, IPersonalityPresetQueries queries)
    {
        var domains = await queries.GetAllAsync();
        return GetAllPersonalityPresets.Success(domains);
    }
}
