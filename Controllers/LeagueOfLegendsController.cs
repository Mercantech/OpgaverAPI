using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpgaverAPI.Models;
using OpgaverAPI.Context;

namespace OpgaverAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeagueOfLegendsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public LeagueOfLegendsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: /LeagueOfLegends
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LolChampion>>> GetChampions()
        {
            return await _context.Champions.ToListAsync();
        }

        // GET: /LeagueOfLegends/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LolChampion>> GetChampion(int id)
        {
            var champion = await _context.Champions.FindAsync(id);

            if (champion == null)
            {
                return NotFound();
            }

            return champion;
        }

        // GET: /LeagueOfLegends/name/{name}
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<LolChampion>>> GetChampionsByName(string name)
        {
            var champions = await _context.Champions
                .Where(c => c.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            if (!champions.Any())
            {
                return NotFound($"No champions found with name containing '{name}'");
            }

            return champions;
        }

        // GET: /LeagueOfLegends/class/{className}
        [HttpGet("class/{className}")]
        public async Task<ActionResult<IEnumerable<LolChampion>>> GetChampionsByClass(string className)
        {
            var champions = await _context.Champions
                .Where(c => c.Classes != null && c.Classes.Any(cl => cl.ToLower().Contains(className.ToLower())))
                .ToListAsync();

            if (!champions.Any())
            {
                return NotFound($"No champions found in class '{className}'");
            }

            return champions;
        }

        // GET: /LeagueOfLegends/classes
        [HttpGet("classes")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllClasses()
        {
            var allClasses = await _context.Champions
                .Where(c => c.Classes != null)
                .SelectMany(c => c.Classes)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            if (!allClasses.Any())
            {
                return NotFound("No champion classes found");
            }

            return allClasses;
        }

        private bool ChampionExists(string id)
        {
            return _context.Champions.Any(e => e.Id == id);
        }
    }
}

// Disabled Endpoints

// POST:
/*LeagueOfLegends
       [HttpPost]
       public async Task<ActionResult<LolChampion>> CreateChampion(LolChampion champion)
       {
           _context.Champions.Add(champion);
           await _context.SaveChangesAsync();

           return CreatedAtAction(nameof(GetChampion), new { id = champion.Id }, champion);
       }
*/


/*
// DELETE: /LeagueOfLegends/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChampion(int id)
        {
            var champion = await _context.Champions.FindAsync(id);
            if (champion == null)
            {
                return NotFound();
            }

            _context.Champions.Remove(champion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

// PUT: /LeagueOfLegends/5 (Disabled)
/*
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChampion(string id, LolChampion champion)
        {
            if (id != champion.Id)
            {
                return BadRequest();
            }

            _context.Entry(champion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChampionExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }
*/
