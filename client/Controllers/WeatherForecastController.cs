using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace client.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://server:5002/WeatherForecast");

        Console.WriteLine(response.StatusCode);
        if(response.StatusCode == HttpStatusCode.OK)
        {
            var res = await response.Content.ReadAsStringAsync();
            return Ok(res);
        }

        return StatusCode((int)response.StatusCode);
    }
}
