namespace Bunker.AccountService.Api.Middlewares;

internal class ExceptionToHttpErrorMiddleware(RequestDelegate next, ILogger<ExceptionToHttpErrorMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (BadHttpRequestException ex)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            ctx.Response.ContentType = "application/json";
            var payload = string.IsNullOrEmpty(ex.Message) ? null : new { error = ex.Message };
            await ctx.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            var payload = new { error = "InternalServerError" };
            await ctx.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(payload));
        }
    }
}
