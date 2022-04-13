using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        readonly Settings _settings;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<Settings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        [HttpGet]
        [Route("GetWeatherInfo")]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherInfo([FromQuery] string latitude = null, [FromQuery] string longitude = null, [FromQuery] string cityKey = null, [FromQuery]  bool useMetric = false)
        {
            _logger.Log(LogLevel.Information, "Getting Weather Info for city key: ", cityKey);
            if ((String.IsNullOrEmpty(latitude) || String.IsNullOrEmpty(longitude)))
            {
                if (String.IsNullOrEmpty(cityKey)) { 
                    var resp = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Error, No CityKey or Lat,Lon provided"),
                        ReasonPhrase = "City Key or Lat,Lon must be provided"
                    };
                    _logger.Log(LogLevel.Warning, "Invalid parameters passed to GetWeatherInfo call.");
                    throw new System.Web.Http.HttpResponseException(resp);
                }
            }
            var dailyForecastBuilder = new UriBuilder($"{_settings.ForecastAPIUrl}{cityKey}");
            var query = HttpUtility.ParseQueryString(dailyForecastBuilder.Query);
            query["apikey"] =  _settings.APIKey;
            if (useMetric)
            {
                query["metric"] = "true";
            }
            dailyForecastBuilder.Query = query.ToString();
            HttpResponseMessage response = await client.GetAsync(dailyForecastBuilder.Uri);
            if (response.IsSuccessStatusCode)
            {
                string oneDayForecastResponse = await response.Content.ReadAsStringAsync();
                Forecast oneDayForecast = JsonConvert.DeserializeObject<Forecast>(oneDayForecastResponse);

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
