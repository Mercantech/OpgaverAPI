using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpgaverAPI.Models;

namespace OpgaverAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Country> _countries;

        public CountriesController(IMongoDatabase database)
        {
            _database = database;
            _countries = database.GetCollection<Country>("countries");
            Console.WriteLine($"Connected to collection: {_countries.CollectionNamespace}"); // Debug log
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                Console.WriteLine("Attempting to fetch all countries"); // Debug log
                var countries = await _countries.Find(_ => true).ToListAsync();
                Console.WriteLine($"Found {countries.Count} countries"); // Debug log
                return Ok(countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Debug log
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var country = await _countries.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (country == null)
                return NotFound();
            return Ok(country);
        }

        [HttpGet("name/{commonName}")]
        public async Task<IActionResult> GetByCommonName(string commonName)
        {
            try
            {
                Console.WriteLine($"Searching for country: {commonName}"); // Debug log
                var filter = Builders<Country>.Filter.Eq("name.common", commonName);
                var country = await _countries.Find(filter).FirstOrDefaultAsync();
                Console.WriteLine($"Found country: {country?.Name.Common ?? "Not found"}"); // Debug log
                if (country == null)
                    return NotFound();
                return Ok(country);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Debug log
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("region/{region}")]
        public async Task<IActionResult> GetByRegion(string region)
        {
            var filter = Builders<Country>.Filter.Eq(x => x.Region, region);
            var countries = await _countries.Find(filter).ToListAsync();
            return Ok(countries);
        }

        [HttpGet("simplified")]
        public async Task<IActionResult> GetSimplifiedCountries()
        {
            try
            {
                var projection = Builders<Country>.Projection
                    .Exclude("_id")
                    .Include(x => x.Name.Common)
                    .Include(x => x.Flags.Png)
                    .Include(x => x.Flags.Svg)
                    .Include(x => x.Flags.Alt)
                    .Include(x => x.Cca3)
                    .Include(x => x.AltSpellings);

                var countries = await _countries
                    .Find(FilterDefinition<Country>.Empty)
                    .Project<SimplifiedCountry>(projection)
                    .ToListAsync();

                return Ok(countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Debug log
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterCountries(
            [FromQuery] string? region,
            [FromQuery] string? subregion,
            [FromQuery] bool? landlocked,
            [FromQuery] string? language,
            [FromQuery] int? minPopulation,
            [FromQuery] int? maxPopulation,
            [FromQuery] bool? unMember)
        {
            try
            {
                var builder = Builders<Country>.Filter;
                var filter = builder.Empty;

                if (!string.IsNullOrEmpty(region))
                    filter &= builder.Eq(x => x.Region, region);

                if (!string.IsNullOrEmpty(subregion))
                    filter &= builder.Eq(x => x.Subregion, subregion);

                if (landlocked.HasValue)
                    filter &= builder.Eq(x => x.Landlocked, landlocked.Value);

                if (!string.IsNullOrEmpty(language))
                    filter &= builder.AnyEq("languages", language);

                if (minPopulation.HasValue)
                    filter &= builder.Gte(x => x.Population, minPopulation.Value);

                if (maxPopulation.HasValue)
                    filter &= builder.Lte(x => x.Population, maxPopulation.Value);

                if (unMember.HasValue)
                    filter &= builder.Eq(x => x.UnMember, unMember.Value);

                var countries = await _countries.Find(filter).ToListAsync();
                Console.WriteLine($"Found {countries.Count} countries"); // Debug log
                return Ok(countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FilterCountries: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}