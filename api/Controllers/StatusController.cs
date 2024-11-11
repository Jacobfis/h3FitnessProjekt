using API.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly AppDBContext _context;

        public StatusController(AppDBContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok("The server is Live");
        }

        [HttpGet("DB")]
        public IActionResult GetStatusDB()
        {
            if (_context.Database.CanConnect())
            {
                return Ok("The database and Server is Live!");
            }
            else return NotFound();
        }
    }
}
