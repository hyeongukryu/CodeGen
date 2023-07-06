using CodeGen.Analysis;
using CodeGen.Generation;

namespace CodeGen.Web;

public static class StartupHelper
{
    public static IServiceCollection AddCodeGen(this IServiceCollection services, bool preserveReferences)
    {
        services.AddEndpointsApiExplorer();
        services.AddScoped<WebRequestHandler>();
        services.AddScoped<ApiAnalyzer>();
        if (preserveReferences)
        {
            services.AddScoped<IReferenceHandlerConfiguration, PreserveReferenceHandlerConfiguration>();
        }
        else
        {
            services.AddScoped<IReferenceHandlerConfiguration, IgnoreCyclesReferenceHandlerConfiguration>();
        }

        return services;
    }

    public static void MapCodeGen(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("code-gen", async context =>
        {
            var assembly = typeof(StartupHelper).Assembly;
            var resource = assembly.GetManifestResourceStream("CodeGen.Web.index.html");
            if (resource == null)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Error: CodeGen.Web.index.html");
                return;
            }

            context.Response.ContentType = "text/html; charset=UTF-8";
            await resource.CopyToAsync(context.Response.Body);
        }).AllowAnonymous();

        endpoints.MapGet("code-gen-api", async context =>
        {
            await using var scope = endpoints.ServiceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<WebRequestHandler>();
            var response = await handler.HandleApiRequest(context.Request);
            await context.Response.WriteAsJsonAsync(response);
        }).AllowAnonymous();
    }
}