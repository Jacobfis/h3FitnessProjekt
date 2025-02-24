using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public QuizzesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Quizzes (Hent alle quizzer)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
            return await _context.Quizzes.Include(q => q.Questions).ThenInclude(q => q.Answers).ToListAsync();
        }

        // GET: api/Quizzes/5 (Hent én quiz)
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuiz(string id)
        {
            var quiz = await _context.Quizzes.Include(q => q.Questions).ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return quiz;
        }

        // POST: api/Quizzes (Tilføj en quiz)
        [Authorize] // Kræver login
        [HttpPost]
        public async Task<ActionResult<Quiz>> PostQuiz(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
        }

        // DELETE: api/Quizzes/5 (Slet en quiz)
        [Authorize] // Kræver login
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(string id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
