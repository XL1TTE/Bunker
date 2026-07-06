using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.CreateLobby;

[WolverineHandler]
public static class CreateLobbyHandler
{
    public static async Task<CreateLobby.Result> Handle(
        CreateLobby command,
        IUnitOfWork uow,
        ILogger<CreateLobby> logger)
    {
        var repository = uow.GetRepository<ILobbyRepository>();

        try
        {
            var visibility = command.IsPublic
                ? PrivacyPolicy.PublicPolicy(command.Password)
                : PrivacyPolicy.PrivatePolicy(command.Password);

            var lobby = Domain.Lobby.Create(capacity: command.Capacity, visibility: visibility);
            lobby.WithHost(AccountId.Create(command.HostId), command.Nickname);

            foreach (var packId in command.SelectedPackIds)
                lobby.AddCardPack(CardPackId.Create(Guid.Parse(packId)));

            repository.Add(lobby);
            await uow.SaveChangesAsync();
            return CreateLobby.Success(lobby);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create lobby (capacity={Capacity}, isPublic={IsPublic})", command.Capacity, command.IsPublic);
            return CreateLobby.Failure("Failed to create lobby.");
        }
    }
}