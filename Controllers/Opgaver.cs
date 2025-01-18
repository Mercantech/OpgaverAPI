using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace OpgaverAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpgaverController : ControllerBase
    {
        private readonly ILogger<OpgaverController> _logger;
        private static int _dieselCallCount = 0;
        private static int _miles95CallCount = 0;
        private static int _bilbasenCallCount = 0;
        private static int _wordleCallCount = 0;
        private static int _countriesCallCount = 0;

        public OpgaverController(ILogger<OpgaverController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Diesel")]
        public IActionResult GetDiesel()
        {
            _dieselCallCount++;
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Diesel endpoint called by IP: {IpAddress}, User-Agent: {UserAgent}. Total calls: {CallCount}", 
                ipAddress, userAgent, _dieselCallCount);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "diesel.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            return Content(jsonData, "application/json");
        }
        [HttpGet("Miles95")]
        public IActionResult GetMiles95()
        {
            _miles95CallCount++;
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Miles95 endpoint called by IP: {IpAddress}, User-Agent: {UserAgent}. Total calls: {CallCount}", 
                ipAddress, userAgent, _miles95CallCount);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "miles95.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            return Content(jsonData, "application/json");
        }
        [HttpGet("Bilbasen")]
        public IActionResult GetBilbasen()
        {
            _bilbasenCallCount++;
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Bilbasen endpoint called by IP: {IpAddress}, User-Agent: {UserAgent}. Total calls: {CallCount}", 
                ipAddress, userAgent, _bilbasenCallCount);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "bilbasen.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            return Content(jsonData, "application/json");
        }
        [HttpGet("Wordle")]
        public IActionResult GetWordleWords()
        {
            _wordleCallCount++;
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Wordle endpoint called by IP: {IpAddress}, User-Agent: {UserAgent}. Total calls: {CallCount}", 
                ipAddress, userAgent, _wordleCallCount);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "wordle.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            return Content(jsonData, "application/json");
        }
        [HttpGet("CountriesRAW")]
        public IActionResult GetCountries()
        {
            _countriesCallCount++;
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("CountriesRAW endpoint called by IP: {IpAddress}, User-Agent: {UserAgent}. Total calls: {CallCount}", 
                ipAddress, userAgent, _countriesCallCount);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "countries.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            return Content(jsonData, "application/json");
        }
        [HttpGet("Stats")]
        public IActionResult GetStats()
        {
            var stats = new
            {
                DieselCalls = _dieselCallCount,
                Miles95Calls = _miles95CallCount,
                BilbasenCalls = _bilbasenCallCount,
                WordleCalls = _wordleCallCount,
                CountriesCalls = _countriesCallCount
            };

            return Ok(stats);
        }
    }
}
