using Fast.Test.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Test.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ITestService _testService;

    public ILogger<WeatherForecastController> Logger1 { get; }

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ITestService testService)
    {
        Logger1 = logger;
        _testService = testService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var a = _testService.Test();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();
    }
}