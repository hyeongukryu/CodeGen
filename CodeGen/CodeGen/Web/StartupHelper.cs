namespace CodeGen.Web;

public static class StartupHelper
{
    public static IServiceCollection AddCodeGen(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddScoped<WebRequestHandler>();
        return services;
    }

    public static void MapCodeGen(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("code-gen", async () =>
        {
            await using var scope = endpoints.ServiceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<WebRequestHandler>();
            return await handler.HandleWebAppRequest();
        });

        endpoints.MapGet("code-gen-api", async () =>
        {
            await using var scope = endpoints.ServiceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<WebRequestHandler>();
            return await handler.HandleApiRequest();
        });
    }
}