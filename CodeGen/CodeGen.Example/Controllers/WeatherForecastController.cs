using CodeGen.Example.Data;
using Microsoft.AspNetCore.Mvc;
using NodaTime.Extensions;

namespace CodeGen.Example.Controllers;

[ApiController]
[Route("weather-forecast")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet("{count:int}/{temp:int}")]
    public async Task<WeatherForecast[]> Get([FromQuery] long value,
        [FromRoute] int count, [FromRoute] int temp)
    {
        await Task.Delay(0);
        return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateTimeOffset.Now.AddDays(index).ToInstant(),
                TemperatureC = Random.Shared.Next(-20, 55) + temp,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                Value = value
            })
            .ToArray();
    }
}