using System.Reflection;

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
        endpoints.MapGet("code-gen", async context =>
        {
            var assembly = typeof(StartupHelper).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream("CodeGen.Web.index.html");
            if (resource == null)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Error: CodeGen.Web.index.html");
                return;
            }

            context.Response.ContentType = "text/html; charset=UTF-8";
            await resource.CopyToAsync(context.Response.Body);
        });

        endpoints.MapGet("code-gen-api", async context =>
        {
            await using var scope = endpoints.ServiceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<WebRequestHandler>();
            var response = await handler.HandleApiRequest(context.Request);
            await context.Response.WriteAsJsonAsync(response);
        });
    }
}