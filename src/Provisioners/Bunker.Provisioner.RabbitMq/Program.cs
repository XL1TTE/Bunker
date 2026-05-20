using ImTools;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(c => c.AddConsole());

builder.Services.AddWolverine(options =>
{
    var mq = options.UseRabbitMqUsingNamedConnection("rabbit-mq").AutoProvision();
    
    mq.DeclareExchange("account-updates", e => 
    { 
        e.ExchangeType = ExchangeType.Fanout; 
        
        e.BindQueue("lobby-service-account-updates");
        e.BindQueue("read-service-account-updates");
    });
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogDebug("Starting RabbitMq provisioning...");
    await app.StartAsync();
}
catch
{
    logger.LogError("RabbitMq provisioning completed with error...");
}
finally
{
    logger.LogDebug("RabbitMq provisioning completed...");
    await app.StopAsync();
}
