using NodaTime;

namespace CodeGen.Example.Data;

public class WeatherForecast
{
    public Instant Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public long Value { get; set; }
}