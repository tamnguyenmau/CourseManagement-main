using linq.model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using querry.dto.model;
using System.Security.Claims;

namespace linq.Controllers
{

    public class RoomCourseController : Controller
    {
        private readonly PostgresContext _dbContext;

        public RoomCourseController(PostgresContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IEnumerable<RoomCourse> Get()
        {
            var mydata = _dbContext.RoomCourses.ToList();
            return mydata;
        }
        [HttpGet("api/RoomCourses/{id}")]
        public async Task<ActionResult<IEnumerable<RoomCourse>>> Get(string roomId)
        {
            var check = _dbContext.RoomCourses.Where(c => c.RoomId == roomId)
                .Select(c => new
                {
                    c.RoomId,
                    c.CreateDate,
                    c.CourseId,
                    c.Room
                })
                .ToList();

            return Ok(check);
        }
        [HttpGet("api/RoomCourses/intervals")]
        public async Task<IActionResult> getroombydate(string id, DateTime startday, DateTime endday)
        {
            var checkroom = _dbContext.RoomCourses
                .Any(c => c.RoomId == id && c.Course.StartDate <= endday && c.Course.EndDate >= startday);
            return Ok(checkroom);
        }
        //[HttpGet("checkStudentInCourseroom")]
        //public async Task<IActionResult> checkstudent( DateTime startday, DateTime endday)
        //{
        //    IQueryable<CourseStudent> getstudent = _dbContext.CourseStudents;
        //    var checkroom = _dbContext.RoomCourses
        //        .Where(c=>c.Course.StartDate <= endday && c.Course.EndDate >= startday)
        //        .Select(d => new
        //        {
        //            roomCourseId = d.Course.CourseId,
        //        })
        //        .ToList();
        //        return Ok(checkroom);
        //}

        [HttpGet("api/RoomCourses/total-student")]
        public async Task<IActionResult> checkstudent(DateTime startday, DateTime endday)
        {
            var mydata = _dbContext.RoomCourses
                .Where(e => e.Course.StartDate <= endday && e.Course.EndDate >= startday)
                .GroupJoin(_dbContext.CourseStudents,
                t1 => t1.CourseId,
                t2 => (t2.CourseId),
                (t1, t2) => new
                {
                    roomCourseId = t1.RoomCourseId,
                    courseId = t1.CourseId,
                    totalStudent = t2.Count()
                })
                .Select(result => new
                {
                    result.roomCourseId,
                    result.courseId,
                    result.totalStudent,
                })
                .ToList();
            return Ok(mydata);
        }
        [HttpPost("api/RoomCourses")]
        public async Task<ActionResult<IEnumerable<Room>>> Post([FromBody] RoomCourseReq roomcourse)
        {
            var roomcourses = new RoomCourse
            {
                //RoomId = Guid.NewGuid().ToString(),
                RoomCourseId = roomcourse.RoomCourseId,
                RoomId = roomcourse.RoomId,
                CourseId = roomcourse.CourseId,
                CreateDate = DateTime.UtcNow,
                CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            await _dbContext.RoomCourses.AddAsync(roomcourses);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction("GetRoomCourses", new { id = roomcourses.RoomCourseId }, roomcourses);
        }
        [HttpPut("api/RoomCourses/{id}")]
        public async Task<ActionResult<IEnumerable<RoomCourse>>> Put(string id, [FromBody] RoomCourse roomCourse)
        {
            if (id != roomCourse.RoomCourseId)
            {
                return BadRequest();
            }
            _dbContext.Entry(roomCourse).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomCourseExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }
        [HttpDelete("api/RoomCourses/{id}")]
        public async Task<ActionResult<IEnumerable<RoomCourse>>> Delete(string id)
        {
            var roomCourse = await _dbContext.RoomCourses.FindAsync(id);
            if (roomCourse == null)
            {
                return NotFound();
            }
            _dbContext.RoomCourses.Remove(roomCourse);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private bool RoomCourseExists(string id)
        {
            return _dbContext.RoomCourses.Any(e => e.RoomCourseId == id);
        }
    }
}
