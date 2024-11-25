using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ActivityLogController(AppDBContext context)
        {
            _context = context;
        }

        // POST: api/ActivityLog
        [HttpPost]
        public async Task<IActionResult> PostActivityLog([FromBody] ActivityLogCreateDTO activityLogDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO til model
            var activityLog = new ActivityLog
            {
                Id = activityLogDto.Id,
                Date = activityLogDto.Date,
                Steps = activityLogDto.Steps,
                Distance = activityLogDto.Distance,
                Calories = activityLogDto.Calories,
                Duration = activityLogDto.Duration,
                Type = activityLogDto.Type
            };

            // Gem i databasen
            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            // Returner det gemte objekt med status 201 Created
            return CreatedAtAction(nameof(GetActivityLog), new { id = activityLog.Id }, activityLog);
        }

        // GET: api/ActivityLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs()
        {
            return await _context.ActivityLogs.ToListAsync();
        }

        // GET: api/ActivityLog/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityLog>> GetActivityLog(string id)
        {
            var activityLog = await _context.ActivityLogs.FindAsync(id);

            if (activityLog == null)
            {
                return NotFound();
            }

            return activityLog;
        }
    }
}
