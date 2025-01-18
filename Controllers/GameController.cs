using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpgaverAPI.Context;
using OpgaverAPI.Models;
using Microsoft.AspNetCore.Authorization;
namespace OpgaverAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly AppDBContext _context;

        public GameController(AppDBContext context)
        {
            _context = context;
        }

        // Opret nyt spil
        [Authorize(Roles = "Mags")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] Game game)
        {
            try
            {
                await _context.Games.AddAsync(game);
                await _context.SaveChangesAsync();
                return Ok(game);
            }
            catch (Exception ex)
            {
                return BadRequest($"Kunne ikke oprette spillet: {ex.Message}");
            }
        }

        // Tilføj highscore
        [Authorize]
        [HttpPost("highscore")]
        public async Task<IActionResult> AddHighscore([FromBody] AddHighscoreDto dto)
        {
            try
            {
                var game = await _context.Games
                    .FirstOrDefaultAsync(g => g.Name.ToLower() == dto.GameName.ToLower());

                if (game == null)
                    return NotFound($"Spillet '{dto.GameName}' blev ikke fundet");

                var highscore = new Highscore
                {
                    Score = dto.Score,
                    UserId = dto.UserId,
                    GameId = game.Id,
                    AchievedAt = DateTime.UtcNow
                };

                await _context.Highscores.AddAsync(highscore);
                await _context.SaveChangesAsync();
                return Ok(highscore);
            }
            catch (Exception ex)
            {
                return BadRequest($"Kunne ikke gemme highscore: {ex.Message}");
            }
        }

        // Hent top 10 highscores for et specifikt spil
        [HttpGet("{gameName}/top10")]
        public async Task<IActionResult> GetTop10Highscores(string gameName)
        {
            try
            {
                var highscores = await _context.Highscores
                    .Include(h => h.User)
                    .Include(h => h.Game)
                    .Where(h => h.Game.Name.ToLower() == gameName.ToLower())
                    .OrderByDescending(h => h.Score)
                    .Take(10)
                    .Select(h => new
                    {
                        h.Score,
                        h.AchievedAt,
                        PlayerName = h.User.Username,
                        GameName = h.Game.Name
                    })
                    .ToListAsync();

                return Ok(highscores);
            }
            catch (Exception ex)
            {
                return BadRequest($"Kunne ikke hente highscores: {ex.Message}");
            }
        }

        // Hent Score for en specifik bruger
        [Authorize]
        [HttpGet("score/{userId}")]
        public async Task<IActionResult> GetScore(string userId)
        {
            var score = await _context.Highscores.FirstOrDefaultAsync(h => h.UserId == userId);
            if (score == null)
                return NotFound($"Ingen score fundet for bruger {userId}");
            return Ok(score);
        }

        // Hent top 5 og score for en specifik bruger
        [Authorize]
        [HttpGet("top5/{userId}")]
        public async Task<IActionResult> GetTop5AndScore(string userId)
        {
            try
            {
                // Hent alle highscores sorteret efter score
                var allHighscores = await _context.Highscores
                    .Include(h => h.User)
                    .OrderByDescending(h => h.Score)
                    .Select(h => new
                    {
                        Position = 0, // Vil blive udfyldt senere
                        h.Score,
                        h.AchievedAt,
                        PlayerName = h.User.Username,
                        IsRequestedUser = h.UserId == userId
                    })
                    .ToListAsync();

                // Tilføj position til hver score
                for (int i = 0; i < allHighscores.Count; i++)
                {
                    allHighscores[i] = allHighscores[i] with { Position = i + 1 };
                }

                // Find brugerens position
                var userScore = allHighscores.FirstOrDefault(h => h.IsRequestedUser);

                // Hent top 5
                var top5 = allHighscores.Take(5).ToList();

                // Hvis brugeren ikke er i top 5, tilføj deres score
                if (userScore != null && !top5.Any(h => h.IsRequestedUser))
                {
                    top5.Add(userScore);
                }

                return Ok(new
                {
                    Highscores = top5,
                    TotalPlayers = allHighscores.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Kunne ikke hente highscores: {ex.Message}");
            }
        }
    }

    public class AddHighscoreDto
    {
        public required string GameName { get; set; }
        public required string UserId { get; set; }
        public required int Score { get; set; }
    }
}