using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Transfers;

[Mapper]
[UseStaticMapper(typeof(BunkerCardToTransferMapperExtensions))]
public static partial class BunkerCardToTransferMapper
{
    [MapProperty(nameof(Domain.BunkerCard.PublicId), nameof(Transfer.BunkerCard.Id))]
    public static partial Transfer.BunkerCard ToTransferObject(this Domain.BunkerCard bunkerCard);
}

internal static class BunkerCardToTransferMapperExtensions
{
    public static Domain.BunkerCard.Id MapId(this Guid bunkerCardId) => Domain.BunkerCard.Id.Create(bunkerCardId);
    public static Guid MapId(this Domain.BunkerCard.Id bunkerCardId) => bunkerCardId.Value;
}