using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Context;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDeviceController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserDeviceController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/UserDevice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDevice>>> GetUserDevices()
        {
            return await _context.UserDevice
                .Include(ud => ud.user)
                .Include(ud => ud.device)
                .ToListAsync();
        }

        // GET: api/UserDevice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDevice>> GetUserDevice(string id)
        {
            var userDevice = await _context.UserDevice
                .Include(ud => ud.user)
                .Include(ud => ud.device)
                .FirstOrDefaultAsync(ud => ud.id == id);

            if (userDevice == null)
            {
                return NotFound();
            }

            return userDevice;
        }

        // PUT: api/UserDevice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDevice(string id, UserDevice userDevice)
        {
            if (id != userDevice.id)
            {
                return BadRequest();
            }

            _context.Entry(userDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserDevice
        [HttpPost]
        public async Task<ActionResult<UserDevice>> PostUserDevice(CreateUserDeviceDTO userDeviceDTO)
        {
            var userDevice = await MapDTOToRelation(userDeviceDTO);

            _context.UserDevice.Add(userDevice);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserDeviceExists(userDevice.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetUserDevice), new { id = userDevice.id }, userDevice);
        }

        // DELETE: api/UserDevice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDevice(string id)
        {
            var userDevice = await _context.UserDevice.FindAsync(id);
            if (userDevice == null)
            {
                return NotFound();
            }

            _context.UserDevice.Remove(userDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDeviceExists(string id)
        {
            return _context.UserDevice.Any(e => e.id == id);
        }

        private async Task<UserDevice> MapDTOToRelation(CreateUserDeviceDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.userId);
            if (user == null) throw new ArgumentException("User not found");

            var device = await _context.Devices.FindAsync(dto.deviceId);
            if (device == null) throw new ArgumentException("Device not found");

            return new UserDevice
            {
                id = Guid.NewGuid().ToString("N"),
                userId = dto.userId,
                deviceId = dto.deviceId
            };
        }
    }
}
