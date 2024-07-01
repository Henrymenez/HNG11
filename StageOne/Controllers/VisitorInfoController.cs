using IPinfo;
using IPinfo.Models;
using Microsoft.AspNetCore.Mvc;
using StageOne.Domain.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace StageOne.Controllers
{

    [ApiController]
    [Route("api/hello")]
    public class VisitorInfoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IPinfoClient _client;
        private readonly string token = "e364ac0f4bf2d3";
        private readonly string weatherId = "d17ff1ac776612468f1d56739c67a897";
        public VisitorInfoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _client = new IPinfoClient.Builder()
                .AccessToken(token)
                .Build();
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetVisitorInfo([FromQuery][Required] string visitor_name)
        {

            // Get the client's IP address
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            // Get the location information based on IP
            var locationResponse = await _httpClient.GetAsync($"https://ipinfo.io/{ip}/geo?token={token}");
            if (!locationResponse.IsSuccessStatusCode)
            {
                return BadRequest("Unable to get location information.");
            }

            var locationJsonResponse = await locationResponse.Content.ReadAsStringAsync();
            var locationInfo = JsonSerializer.Deserialize<LocationInfo>(locationJsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (locationInfo == null)
            {
                return BadRequest("Unable to parse location information.");
            }

            // Get the weather information based on location
            var weatherResponse = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={locationInfo.Latitude}&lon={locationInfo.Longitude}&units=metric&appid={weatherId}");
            if (!weatherResponse.IsSuccessStatusCode)
            {
                return BadRequest("Unable to get weather information.");
            }

            var weatherJsonResponse = await weatherResponse.Content.ReadAsStringAsync();
            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(weatherJsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherInfo == null)
            {
                return BadRequest("Unable to parse weather information.");
            }

            // Prepare the response
            var result = new
            {
                client_ip = ip,
                location = locationInfo.City,
                greeting = $"Hello, {visitor_name}!, the temperature is {weatherInfo.Main.Temp} degrees Celsius in {locationInfo.City}"
            };

            return Ok(result);
        }
    }


}

