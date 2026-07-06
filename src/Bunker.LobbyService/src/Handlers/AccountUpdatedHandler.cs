using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Messages;
using Bunker.LobbyService.Persistence;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Bunker.LobbyService.Handlers;

[WolverineHandler]
public static class AccountUpdatedHandler
{
    public static void Configure(HandlerChain chain)
    {
        chain.OnException<DbException>()
            .ScheduleRetry(1.Seconds(), 5.Seconds(), 15.Seconds()).WithBoundedJitter(0.25)
            .Then
            .MoveToErrorQueue();
    }

    public static async Task Handle(
        AccountUpdated message,
        [FromServices] AccountsDbContext AccountsDb)
    {
        if (string.IsNullOrWhiteSpace(message.id))
            throw new ArgumentException("AccountUpdated received with empty id.");

        var accountId = AccountId.Create(message.id);

        await AccountsDb.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Accounts\" (\"AccountId\") VALUES ({accountId.Value}) ON CONFLICT (\"AccountId\") DO NOTHING");

        Log.Information("Account read-model ensured for {AccountId}", accountId.Value);
    }
}