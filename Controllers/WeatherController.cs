using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace WeatherApi.Controllers
{
    [Route("api/[action]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(ILogger<WeatherController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> CurrentWeather(string city)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://api.openweathermap.org");
                    var response = await client.GetAsync($"/data/2.5/weather?q={city}&appid=780104290d484ec72be8129eef17e671&units=metric");
                    response.EnsureSuccessStatusCode();
                    
                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);
                    
                    return Ok(new
                    {
                        Date = DateTimeOffset.FromUnixTimeSeconds((long)rawWeather.Dt).ToString("dddd, dd MMMM yyyy"),
                        Temp = rawWeather.Main.Temp.ToString() + " C",
                        WindSpeed = rawWeather.Wind.Speed.ToString() + " km/h",
                        Clouds = rawWeather.Clouds.All.ToString() + " %"

                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
            }
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetForecast(string city)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://api.openweathermap.org");
                    var response = await client.GetAsync($"/data/2.5/forecast?q={city}&appid=780104290d484ec72be8129eef17e671&units=metric");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject<FiveDayForecast>(stringResult);
                    var weatherList = rawWeather.List;
                    var result = weatherList.Select(item => new
                    {
                        Date = DateTimeOffset.FromUnixTimeSeconds((long)item.Dt).ToString("dddd, dd MMMM yyyy"),
                        MinTemp = item.Main.Temp_min.ToString() + " C",
                        MaxTemp = item.Main.Temp_max.ToString() + " C",
                        WindSpeed = item.Wind.Speed.ToString() + " km/h",
                        Clouds = item.Clouds.All.ToString() + " %"
                    }).ToList();
                    return Ok(
                      result
                    );
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
            }
        }
    }
}
