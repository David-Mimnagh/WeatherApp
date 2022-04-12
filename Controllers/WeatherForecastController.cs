using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        static HttpClient client = new HttpClient();

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetWeatherInfo")]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherInfo([FromQuery] string latitude = null, [FromQuery] string longitude = null, [FromQuery] string location = null)
        {
            _logger.Log(LogLevel.Warning, "Location: ", location);
            const string APIKey = "2RSqE3YQGDu0iQo55Kue9C1tU7AhJhMc";
            const string dailyForecastURL = "http://dataservice.accuweather.com/forecasts/v1/daily/1day/";
            if (location !=  null)
            {
                var citySearchBuilder = new UriBuilder("http://dataservice.accuweather.com/locations/v1/cities/search");
                var query = HttpUtility.ParseQueryString(citySearchBuilder.Query);
                query["apikey"] = APIKey;
                query["q"] = location;
                citySearchBuilder.Query = query.ToString();
                HttpResponseMessage response = await client.GetAsync(citySearchBuilder.Uri);
                if (response.IsSuccessStatusCode)
                {
                    string citySearchResponse = await response.Content.ReadAsStringAsync();
                    IList<City> cities = JsonConvert.DeserializeObject<IList<City>>(citySearchResponse);
                    //for simplicity, just assume we meant the first - should really be finetuned with the search
                    var dailyForecastBuilder = new UriBuilder($"{dailyForecastURL}{cities.First().Key}");
                    query = HttpUtility.ParseQueryString(dailyForecastBuilder.Query);
                    query["apikey"] = APIKey;
                    dailyForecastBuilder.Query = query.ToString();
                    response = await client.GetAsync(dailyForecastBuilder.Uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string oneDayForecastResponse = await response.Content.ReadAsStringAsync();
                        Forecast oneDayForecast = JsonConvert.DeserializeObject<Forecast>(oneDayForecastResponse);

                    }
                }
            }
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
