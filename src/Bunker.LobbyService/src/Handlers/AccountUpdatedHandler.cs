
using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Messages;
using Bunker.LobbyService.Persistence;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Bunker.LobbyService.Handlers;

[WolverineHandler]
public static class AccountUpdatedHandler
{
    public static void Configure(HandlerChain chain)
    {
        chain.OnException<Exception>()
            .ScheduleRetry(1.Seconds(), 5.Seconds()).WithBoundedJitter(0.25)
            .Then
            .Requeue().AndPauseProcessing(1.Minutes());
    }

    public static async Task Handle(
        AccountUpdated message, 
        [FromServices] AccountsDbContext AccountsDb)
    {
        Log.Information($"Checking if account id is saved in replica set...");
        
        var accountId = AccountId.Create(Guid.Parse(message.id));
        if(await AccountsDb.Accounts.AnyAsync(x => x.PublicId == accountId)){
            Log.Information($"Account id already saved! Aborting...");
            return;
        }

        Log.Information($"Account id need to be saved! Trying to write in replica set...");
        try
        {
            AccountsDb.Accounts.Add(new AccountReplica(accountId));
            await AccountsDb.SaveChangesAsync();
            Log.Information($"Account id saved successfully...");
        }
        catch(Exception e)
        {
            Log.Information($"Something went wrong when tried to save account in replica set...\n\tError:{e}");
        }
    }
}
