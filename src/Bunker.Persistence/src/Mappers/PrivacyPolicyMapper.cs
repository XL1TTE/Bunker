
using Bunker.Domain.Lobbies;
using Riok.Mapperly.Abstractions;

namespace LobbyService.Persistence.Mappers;

[Mapper]
public static partial class PrivacyPolicyMapper
{
    public static partial PrivacyPolicy ToDomain(this Entities.PrivacyPolicy policy);
    public static partial Entities.PrivacyPolicy ToEntity(this PrivacyPolicy policy);
}
