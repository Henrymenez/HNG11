using IPinfo;
using IPinfo.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace StageOne.Controllers
{

    [ApiController]
    [Route("api/hello")]
    public class VisitorInfoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IPinfoClient _client;

        public VisitorInfoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            string token = "e364ac0f4bf2d3";
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

            /*// Get the location information based on IP
            var locationInfo = await _httpClient.GetAsync($"https://ipapi.co/{clientIp}/city/");*/
            // making API call
            //string ip = "216.239.36.21";
            IPResponse ipResponse = await _client.IPApi.GetDetailsAsync(ip);
            if (ipResponse != null)
            {
                
                var response = new
                {
                    client_ip = ipResponse.IP,
                    location = ipResponse.City,
                    greeting = $"Hello, {visitor_name}!, the temperature is 11 degrees Celsius in {ipResponse.City}"
                };

                return Ok(response);

            }

            return BadRequest("Unable to get location information.");
        }
    }


}

