using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Transfers;

[Mapper]
[UseStaticMapper(typeof(BotPersonalityPresetMapperExtensions))]
public static partial class BotPersonalityPresetMapper
{
    [MapProperty(nameof(Domain.PersonalityPreset.PublicId), nameof(Transfer.PersonalityPreset.Id))]
    public static partial Transfer.PersonalityPreset ToTransferObject(this Domain.PersonalityPreset botPersonaltyPreset);
}

internal static class BotPersonalityPresetMapperExtensions
{
    public static Domain.PersonalityPreset.Id MapId(this Guid botPersonaltyPresetId) => Domain.PersonalityPreset.Id.Create(botPersonaltyPresetId);
    public static Guid MapId(this Domain.PersonalityPreset.Id botPersonaltyPresetId) => botPersonaltyPresetId.Value;
}
