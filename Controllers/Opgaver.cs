using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

namespace OpgaverAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpgaverController : ControllerBase
    {
        [HttpGet("Diesel")]
        public IActionResult GetDiesel()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "diesel.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            var dieselData = JsonSerializer.Deserialize<object>(jsonData);

            return Ok(dieselData);
        }
        [HttpGet("Miles95")]
        public IActionResult GetMiles95()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "miles95.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            var dieselData = JsonSerializer.Deserialize<object>(jsonData);

            return Ok(dieselData);
        }
        [HttpGet("Bilbasen")]
        public IActionResult GetBilbasen()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "bilbasen.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            var dieselData = JsonSerializer.Deserialize<object>(jsonData);

            return Ok(dieselData);
        }
        [HttpGet("Wordle")]
        public IActionResult GetWordleWords()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JSON", "wordle.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            var dieselData = JsonSerializer.Deserialize<object>(jsonData);

            return Ok(dieselData);
        }
    }
}
