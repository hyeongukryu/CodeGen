using System.Text.Json.Serialization;
using CodeGen.Web;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
    });
builder.Services.AddCodeGen();

var app = builder.Build();

app.UseRouting();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.MapControllers();
app.MapCodeGen();

app.Run();