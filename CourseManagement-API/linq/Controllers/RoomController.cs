using linq.model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using querry.dto.model;
using System.Security.Claims;

namespace linq.Controllers
{

    public class RoomController : Controller
    {
        private readonly PostgresContext _dbContext;

        public RoomController(PostgresContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("api/Rooms")]
        public async Task<IActionResult> Get()
        {
            var mydata = _dbContext.Rooms.ToList();
            return Ok(mydata);
        }
        [HttpGet("api/Rooms/{id}")]
        public async Task<ActionResult<IEnumerable<Room>>> get(string id)
        {
            var room = await _dbContext.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        [HttpPost("api/Rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> Post([FromBody] RoomReq room)
        {
            var rooms = new Room
            {
                //RoomId = Guid.NewGuid().ToString(),
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Description = room.Description,
                CreateDate = DateTime.UtcNow,
                CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            await _dbContext.Rooms.AddAsync(rooms);
            await _dbContext.SaveChangesAsync();
            return Ok(rooms);
        }
        [HttpPut("api/Rooms/{id}")]
        public async Task<ActionResult<IEnumerable<Room>>> Put(string id, [FromBody] RoomReq room)
        {

            var rooms = await _dbContext.Rooms.FirstOrDefaultAsync(d =>d.RoomId == id);
           if (rooms != null)
            {
                rooms.RoomName = room.RoomName;
                rooms.Description = room.Description;
                rooms.UpdateDate = DateTime.UtcNow;
                rooms.UpdatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
             await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("api/Rooms/{id}")]
        public async Task<ActionResult<IEnumerable<Room>>> Delete(string id)
        {
            var room = await _dbContext.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        private bool RoomsExist(string id)
        {
            return _dbContext.Rooms.Any(e => e.RoomId == id);
        }
    }
}
