using System.Text.Json;
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
        IMessageContext messaging,
        IUnitOfWork uow,
        ILogger<CreateLobby> logger)
    {
        var repository = uow.GetRepository<ILobbyRepository>();

        try
        {
            var lobby = Domain.Lobby.Create(
                capacity: command.Capacity,
                visibility: command.Visible ? PrivacyPolicy.PublicPolicy(password: command.LobbyPassword) : PrivacyPolicy.PrivatePolicy(password: command.LobbyPassword)
            );
            lobby.WithHost(AccountId.Create(Guid.Parse(command.HostId)), command.Nickname);
            repository.Add(lobby);
            
            await uow.SaveChangesAsync();
            return CreateLobby.Success(lobby);
        }
        catch
        {
            logger.LogError("Failed to create lobby with command: {@Command}", JsonSerializer.Serialize(command));
            return CreateLobby.Failure("Failed to create lobby.");
        }
    }
}
