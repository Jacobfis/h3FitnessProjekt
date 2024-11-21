using Microsoft.AspNetCore.Mvc;
using API.Models; // For din ActivityLog model og DTO
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using API.Context;

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

        // POST: api/activitylog
        [HttpPost]
        public async Task<IActionResult> PostActivityLog([FromBody] ActivityLogCreateDTO activityLogDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapper DTO til model
            var activityLog = new ActivityLog
            {
                Id = activityLogDto.Id,
                Date = activityLogDto.Date,
                Steps = activityLogDto.Steps,
                Distance = activityLogDto.Distance,
                Duration = activityLogDto.Duration,
                Type = activityLogDto.Type
            };

            // Gem data i databasen
            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            // Returner det gemte objekt med status 201 (Created)
            return CreatedAtAction("GetActivityLog", new { id = activityLog.Id }, activityLog);
        }

        // GET: api/activitylog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs()
        {
            return await _context.ActivityLogs.ToListAsync();
        }

        // GET: api/activitylog/{id}
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
