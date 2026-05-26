
using Bunker.ContentService.Persistence.Entities;
using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(BotPersonalityPresetMapperExtensions))]
public static partial class BotPersonalityPresetMapper
{
    public static partial Domain.PersonalityPreset ToDomain(this PersonalityPreset botPersonaltyPreset);

    public static partial PersonalityPreset ToEntity(this Domain.PersonalityPreset botPersonaltyPreset);

    public static partial void ApplyUpdate([MappingTarget] this PersonalityPreset entity, PersonalityPreset botPersonaltyPreset);
}

internal static class BotPersonalityPresetMapperExtensions
{
    public static Domain.PersonalityPreset.Id MapId(this Guid botPersonaltyPresetId) => Domain.PersonalityPreset.Id.Create(botPersonaltyPresetId);
    public static Guid MapId(this Domain.PersonalityPreset.Id botPersonaltyPresetId) => botPersonaltyPresetId.Value;
}
