using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpgaverAPI.Models;

namespace OpgaverAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentQuizController : ControllerBase
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<StudentQuiz> _studentQuizzes;

        public StudentQuizController(IMongoDatabase database)
        {
            _database = database;
            _studentQuizzes = database.GetCollection<StudentQuiz>("studentQuizzes");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var studentQuizzes = await _studentQuizzes.Find(_ => true).ToListAsync();

            var overviews = studentQuizzes.Select(quiz => new StudentQuizOverviewDTO
            {
                Id = quiz.Id,
                MadeBy = quiz.MadeBy,
                Topic = quiz.Topic,
                NumberOfQuestions = quiz.Questions?.Count ?? 0,
                CreatedAt = quiz.CreatedAt ?? DateTime.MinValue
            }).ToList();

            return Ok(overviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var studentQuiz = await _studentQuizzes.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (studentQuiz == null)
                return NotFound();
            return Ok(studentQuiz);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentQuizPostDTO studentQuiz)
        {
            var newStudentQuiz = new StudentQuiz
            {
                MadeBy = studentQuiz.MadeBy,
                Topic = studentQuiz.Topic,
                Questions = studentQuiz.Questions
            };
            newStudentQuiz.CreatedAt = DateTime.Now;
            newStudentQuiz.UpdatedAt = DateTime.Now;
            await _studentQuizzes.InsertOneAsync(newStudentQuiz);
            return CreatedAtAction(nameof(GetById), new { id = newStudentQuiz.Id }, newStudentQuiz);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, StudentQuiz studentQuiz)
        {
            var existingStudentQuiz = await _studentQuizzes.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (existingStudentQuiz == null)
                return NotFound();
            studentQuiz.Id = existingStudentQuiz.Id;
            await _studentQuizzes.ReplaceOneAsync(s => s.Id == id, studentQuiz);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _studentQuizzes.DeleteOneAsync(s => s.Id == id);
            if (result.DeletedCount == 0)
                return NotFound();
            return NoContent();
        }

        [HttpGet("topic/{topic}")]
        public async Task<IActionResult> GetByTopic(string topic)
        {
            var quizzes = await _studentQuizzes.Find(q => q.Topic == topic).ToListAsync();
            if (!quizzes.Any())
                return NotFound();
            return Ok(quizzes);
        }
    }
}
