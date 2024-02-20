using linq.model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using querry.dto.model;
using System.Security.Claims;

namespace linq.Controllers
{
    public class CourseStudentController : Controller
    {
        private readonly PostgresContext _dbContext;

        public CourseStudentController(PostgresContext dbContext)
        {
            _dbContext = dbContext;
        }
        //[HttpGet("api/CourseStudents")]
        //public async Task<ActionResult<IEnumerable<CourseStudent>>> Get()
        //{

        //    var mydata = _dbContext.CourseStudents
        //        .Select(x => new
        //    {
        //        courseStudentId = x.CourseStudentId,
        //        courseId = x.CourseId,
        //        studentId = x.StudentId,
        //        studentName = x.Student.FullName,
        //        createDate = x.CreateDate,
        //        createdUserId = x.CreatedUserId
        //        })
        //        .ToList();
        //    return Ok(mydata);
        //}

        [HttpGet("api/CourseStudents")]
        public async Task<ActionResult<IEnumerable<CourseStudent>>> GetStudent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var mydata = _dbContext.CourseStudents
                .Where(c=>c.StudentId == userId)
                .Select(x=> new
                {
                    courseStudentId = x.CourseStudentId,
                    courseId = x.CourseId,
                    courseName = x.Course.CourseName,
                    studentId = x.StudentId,
                    studentName = x.Student.FullName,
                    createDate = x.CreateDate
                })
                .ToList();
            return Ok(mydata);
        }

        [HttpGet("api/CourseStudents/fewest-teacher")]
        public async Task<IActionResult> getfewestteacher()
        {
            var fewestTeacher = _dbContext.AspNetUsers
                .Where(c => c.IsTeacher == true)
                .Select(d => new
                {
                    TeacherId = d.Id,
                    TeacherName = d.FullName,
                    Total = d.Courses.Where(e => e.TeacherId == d.Id)
                    .SelectMany(p => p.CourseStudents).Count()
                })
                .OrderBy(e => e.Total)
                //.Take(2)
                .ToList();
            return Ok(fewestTeacher);
        }

        [HttpGet("api/CourseStudents/{id}/students")]
        public async Task<ActionResult<IEnumerable<CourseStudent>>> Get(string studentId)
        {
            var studentcourse = _dbContext.CourseStudents
                .Where(t => t.StudentId == studentId)
                .Select(c => c.Course)
                .ToList();
            return Ok(studentcourse);
        }

        [HttpGet("api/CourseStudents/total-student")]
        public async Task<ActionResult> Gettotal()
        {
            var totalstudent = _dbContext.AspNetUsers
                .Where(c => c.IsStudent == true)
                .Select(d => new
                {
                    StudentId = d.Id,
                    Fullname = d.FullName,
                    Total = ((int)d.CourseStudents.Select(e => (e.Course.EndDate.Value - e.Course.StartDate.Value).TotalHours).Sum()),
                })
                .ToList();
            return Ok(totalstudent);

            //var studentcourse = _dbContext.CourseStudents
            //    .Where(t => t.Student != null)
            //    .Select(c => new
            //    {
            //        id = c.CourseId,
            //        name = c.Student.FullName,
            //        price = c.Course.Price,
            //    })
            //    .GroupBy(c => c.id)
            //    .Select(c => new
            //    {
            //        id = c.Select(c => c.id),
            //        name = c.Select(c => c.name).FirstOrDefault(),
            //        price = c.Sum(c => c.price)
            //    })
            //    .ToList();
            //var result = studentcourse.OrderByDescending(c => c.price).ToList();
            //return Ok(result);
        }

        [HttpPost("api/CourseStudents")]
        public async Task<ActionResult<IEnumerable<CourseStudent>>> Post([FromBody] CourseStudentReq courseStudent)
        {
            
            var coursestudent = new CourseStudent
            {
                CourseStudentId = Guid.NewGuid().ToString(),
                CourseId = courseStudent.CourseId,
                StudentId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreateDate = DateTime.UtcNow,
                CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            var check = await _dbContext.CourseStudents
                .FirstOrDefaultAsync(x=> x.CourseId == courseStudent.CourseId);
            if (check == null)
            {
                var course = _dbContext.Courses.
               Include(t => t.CourseStudents).
               Where(t => t.CourseId == courseStudent.CourseId).
               FirstOrDefault();
                if (course.MaxStudentQuantity.HasValue && course.CourseStudents.Count >= course.MaxStudentQuantity.Value)
                {
                    return BadRequest("Maximum student");
                }
                else
                {
                    await _dbContext.CourseStudents.AddAsync(coursestudent);
                    await _dbContext.SaveChangesAsync();
                    return Ok(coursestudent);
                }
            }
            else
            {
                return BadRequest("duplicate data");
            }
        }
        [HttpPut("api/CourseStudents/{id}")]
        public async Task<ActionResult<IEnumerable<CourseStudent>>> Put(string id, [FromBody] CourseStudentReq coursestudent)
        {
            var coursestudents = await _dbContext.CourseStudents.FirstOrDefaultAsync(c => c.CourseStudentId == id);

            if (coursestudents != null)
            {
                coursestudents.CourseId = coursestudent.CourseId;
                coursestudents.StudentId = coursestudent.StudentId;
                coursestudents.UpdatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                coursestudents.UpdateDate = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("api/CourseStudents/{id}")]
        public async Task<ActionResult<IEnumerable<CourseStudent>>> Delete(string id)
        {
            var Coursestudent = await _dbContext.CourseStudents.FindAsync(id);
            if (Coursestudent == null)
            {
                return NotFound();
            }
            _dbContext.CourseStudents.Remove(Coursestudent);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private bool CoursesStudentExists(string id)
        {
            return _dbContext.CourseStudents.Any(e => e.CourseId == id);
        }
    }
}
