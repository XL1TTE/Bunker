namespace GameStateService.Configuration;

internal static class RedisConfiguration
{
    internal static IHostApplicationBuilder IncludeRedis(this IHostApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
            options.InstanceName = "GameState_";
        });

        return builder;
    }
}
