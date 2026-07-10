using Bunker.ContentService.Persistence.Entities;
using Riok.Mapperly.Abstractions;

namespace Bunker.ContentService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(BunkerCardMapperExtensions))]
public static partial class BunkerCardMapper
{
    public static partial Domain.BunkerCard ToDomain(this BunkerCard bunkerCard);

    public static partial BunkerCard ToEntity(this Domain.BunkerCard bunkerCard);

    public static partial void ApplyUpdate([MappingTarget] this BunkerCard entity, BunkerCard bunkerCard);
}

internal static class BunkerCardMapperExtensions
{
    public static Domain.BunkerCard.Id MapId(this Guid bunkerCardId) => Domain.BunkerCard.Id.Create(bunkerCardId);
    public static Guid MapId(this Domain.BunkerCard.Id bunkerCardId) => bunkerCardId.Value;
}