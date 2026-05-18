using Bunker.Domain.Bots;
using Riok.Mapperly.Abstractions;

namespace Bunker.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(BotPersonalityPresetMapperExtensions))]
public static partial class BotPersonalityPresetMapper
{
    public static partial PersonalityPreset ToDomain(this Entities.PersonalityPreset botPersonaltyPreset);

    public static partial Entities.PersonalityPreset ToEntity(this PersonalityPreset botPersonaltyPreset);

    public static partial void ApplyUpdate([MappingTarget] this Entities.PersonalityPreset entity, PersonalityPreset botPersonaltyPreset);
}

internal static class BotPersonalityPresetMapperExtensions
{
    public static PersonalityPreset.Id MapId(this Guid botPersonaltyPresetId) => PersonalityPreset.Id.Create(botPersonaltyPresetId);
    public static Guid MapId(this PersonalityPreset.Id botPersonaltyPresetId) => botPersonaltyPresetId.Value;
}
