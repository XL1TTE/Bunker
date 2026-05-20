using Riok.Mapperly.Abstractions;

namespace Bunker.LobbyService.Persistence.Entities;

[Mapper]
public static partial class PrivacyPolicyMapper
{
    public static partial Domain.PrivacyPolicy ToDomain(this PrivacyPolicy policy);
    public static partial PrivacyPolicy ToEntity(this Domain.PrivacyPolicy policy);
}
