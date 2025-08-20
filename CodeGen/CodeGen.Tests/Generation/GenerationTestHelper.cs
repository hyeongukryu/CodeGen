using System.Reflection;
using CodeGen.Web;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace CodeGen.Tests.Generation;

internal static class GenerationTestHelper
{
    public static ServiceProvider BuildServiceProvider(bool configureExpectedJsonOptions,
        params Type[] controllerTypes)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var hostEnvironment = new TestHostEnvironment();
        services.AddSingleton<IHostEnvironment>(hostEnvironment);
        services.AddSingleton<IWebHostEnvironment>(hostEnvironment);

        var mvcBuilder = services.AddControllers();
        foreach (var assembly in controllerTypes.Select(type => type.Assembly).Distinct())
        {
            mvcBuilder.AddApplicationPart(assembly);
        }

        mvcBuilder.ConfigureApplicationPartManager(manager =>
        {
            manager.FeatureProviders.Add(new SelectedControllerFeatureProvider(controllerTypes));
        });

        if (configureExpectedJsonOptions)
        {
            mvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.JsonSerializerOptions.NumberHandling =
                    System.Text.Json.Serialization.JsonNumberHandling.WriteAsString |
                    System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
            });
        }

        services.AddCodeGen(true);
        return services.BuildServiceProvider();
    }

    private sealed class TestHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = typeof(GenerationTestHelper).Assembly.GetName().Name!;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = AppContext.BaseDirectory;
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class SelectedControllerFeatureProvider(IEnumerable<Type> controllerTypes)
        : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly ISet<TypeInfo> _controllerTypes = controllerTypes.Select(type => type.GetTypeInfo()).ToHashSet();

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            for (var index = feature.Controllers.Count - 1; index >= 0; index--)
            {
                if (!_controllerTypes.Contains(feature.Controllers[index]))
                {
                    feature.Controllers.RemoveAt(index);
                }
            }

            foreach (var controllerType in _controllerTypes)
            {
                if (!feature.Controllers.Contains(controllerType))
                {
                    feature.Controllers.Add(controllerType);
                }
            }
        }
    }
}
