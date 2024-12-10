using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DevicesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Devices
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<DeviceDataReadDTO>>> GetDevices()
        {
            var devices = await _context.Device.ToListAsync();
            return devices.Select(d => new DeviceDataReadDTO
            {
                DeviceId = d.DeviceId,
                DeviceName = d.DeviceName
            }).ToList();
        }

        // GET: api/Devices/{id}
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<DeviceDataReadDTO>> GetDevice(string id)
        {
            var device = await _context.Device.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return new DeviceDataReadDTO
            {
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName
            };
        }

        // POST: api/Devices
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<DeviceDataReadDTO>> PostDevice([FromBody] DeviceDataCreateDTO deviceDto)
        {
            var device = new Device
            {
                Id = Guid.NewGuid().ToString(), // Sætter en unik værdi for Id
                DeviceId = deviceDto.DeviceId,
                DeviceName = deviceDto.DeviceName
            };

            _context.Device.Add(device);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeviceExists(device.DeviceId))
                {
                    return Conflict("A device with this ID already exists.");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, new DeviceDataReadDTO
            {
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName
            });
        }



        // DELETE: api/Devices/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(string id)
        {
            var device = await _context.Device.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Device.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(string id)
        {
            return _context.Device.Any(e => e.DeviceId == id);
        }
    }
}
