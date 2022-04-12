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
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        static HttpClient client = new HttpClient();

        public CityController(ILogger<CityController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("GetCities")]
        public async Task<IEnumerable<CityCondensed>> GetCities([FromQuery] string location = null)
        {
            _logger.Log(LogLevel.Warning, "Location: ", location);
            //const string APIKey = "2RSqE3YQGDu0iQo55Kue9C1tU7AhJhMc";
            //var citySearchBuilder = new UriBuilder("http://dataservice.accuweather.com/locations/v1/cities/autocomplete");
            //var query = HttpUtility.ParseQueryString(citySearchBuilder.Query);
            //query["apikey"] = APIKey;
            //if (location == null)
            //{
            //    var resp = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            //    {
            //        Content = new StringContent("Error, No Location provided"),
            //        ReasonPhrase = "Location must be provided"
            //    };
            //    throw new System.Web.Http.HttpResponseException(resp);
            //}
            //query["q"] = location;
            //citySearchBuilder.Query = query.ToString();
            //HttpResponseMessage response = await client.GetAsync(citySearchBuilder.Uri);
            //if (!response.IsSuccessStatusCode)
            //{
            //    var resp = new HttpResponseMessage(response.StatusCode)
            //    {
            //        Content = new StringContent("Error Retrieving cities"),
            //        ReasonPhrase = "Bad response from AcuWeather API."
            //    };
            //    throw new System.Web.Http.HttpResponseException(resp);
            //}
            //string citySearchResponse = await response.Content.ReadAsStringAsync();
            //IEnumerable<City> cities = JsonConvert.DeserializeObject<IEnumerable<City>>(citySearchResponse);
            //IEnumerable<CityCondensed> briefCities = cities.Select(city => new CityCondensed() { 
            //                                                Key = city.Key, 
            //                                                Name = $"{city.LocalizedName} - {city.Country.LocalizedName}" 
            //                                            }).ToArray();
            IList<CityCondensed> cityCondenseds = new List<CityCondensed>() { 
                new CityCondensed() {Key="bob", Name="Glasgow - United Kingdom" },
                new CityCondensed() {Key="the", Name="Glasgow - United States" },
                new CityCondensed() {Key="builder", Name="Glasgow - United Kingdom" } };
            return cityCondenseds;
            }
            
        }
}
