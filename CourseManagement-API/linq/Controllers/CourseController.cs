using linq.dto.model;
using linq.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using querry.dto.model;
using System.Security.Claims;

namespace linq.Controllers
{

    public class CourseController : Controller
    {
        private readonly PostgresContext _dbContext;

        public CourseController(PostgresContext dbContext)
        {
            _dbContext = dbContext;
        }
        //check
        [HttpGet("api/Courses")]
        public async Task<ActionResult<IEnumerable<Course>>> getAll()
        {
            var course = _dbContext.Courses
                .Select(x=> new
                {
                    courseId = x.CourseId,
                    courseName = x.CourseName,
                    teacherId = x.TeacherId,
                    teacherName = x.Teacher.FullName,
                    price = x.Price,
                    maxStudentQuantity = x.MaxStudentQuantity,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    createDate = x.CreateDate,
                    createdUserId = x.CreatedUserId,
                    createdUserName = x.Teacher.FullName
                })
                .ToList();
            return Ok(course);
        }
        [HttpGet("api/Courses/{id}")]
        public async Task<IActionResult> GetCourceById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.CourseStudents
                .Where(x=>x.StudentId == userId)
                .Select(x=> new
                {
                    CourseId = x.CourseId,
                    TeacherName = x.Course.Teacher.FullName,
                     CourseName = x.Course.CourseName,
                    StartDate = x.Course.StartDate,
                    EndDate = x.Course.EndDate
                })
                .ToList();
            return Ok(course);
        }
        [HttpGet("api/Courses/maximum-students")]
        public async Task<ActionResult<IEnumerable<Course>>> getmaxstudentquantity()
        {
            var course = _dbContext.Courses.Select(c => new
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                TeacherId = c.TeacherId,
                teacherName = c.Teacher.FullName,
                Price = c.Price,
                MaxStudentQuantity = c.CourseStudents.Count() + "/" + c.MaxStudentQuantity,
                StartDate = c.StartDate,
                EndDate = c.EndDate,

            }).ToList();
            return Ok(course);
        }
        [HttpGet("api/Courses/{id}/teachers")]
        public async Task<IActionResult> getbyid()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.Courses
                .Where(c=>c.TeacherId == userId)
                .Select(x=> new {
                    courseId = x.CourseId,
                    courseName = x.CourseName,
                    description = x.Description,
                    teacherId = x.TeacherId,
                    teacherName = x.Teacher.FullName,
                    price = x.Price,
                    maxStudentQuantity = x.MaxStudentQuantity,
                    startDate = x.StartDate,
                    endDate = x.EndDate,

                })
                .ToList();
            return Ok(course);
        }

        [HttpGet("api/Courses/teacher-unknown")]
        public async Task<IActionResult> getnullteacher()
        {
            var course = await _dbContext.Courses.Where(c => c.TeacherId == null).ToListAsync();
            return Ok(course);
        }
        [HttpPut("api/Courses/teacher-unknown/{id}")]
        public async Task<ActionResult<IEnumerable<Course>>> PutCourseNull(string id)
        {
            var courses = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == id);

            if (courses != null)
            {
                courses.TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            await _dbContext.SaveChangesAsync();
            return Ok(courses);
        }

        [HttpGet("api/Courses/teacher-is-student")]
        public async Task<IActionResult> Getisteacher()
        {
            var course = _dbContext.Courses
                .Where(c => c.CourseStudents
                .Any(c => c.Student.IsTeacher == true))
                .ToList();

            return Ok(course);
        }
        [HttpGet("api/Courses/fewest-course")]
        public async Task<IActionResult> getfewest()
        {
            var mydata = _dbContext.Courses
                .OrderBy(c => c.CourseStudents.Count)
                .Take(2)
                .ToList();
            return Ok(mydata);
        }
        [HttpGet("api/Courses/most-course")]
        public async Task<IActionResult> getmost()
        {
            var mydata = _dbContext.Courses
                .OrderByDescending(c => c.CourseStudents.Count)
                .ToList();
            return Ok(mydata);
        }
        [HttpGet("api/Courses/total-salary-and-time")]
        public async Task<IActionResult> Gettotal()
        {
            var totalPrice = _dbContext.AspNetUsers
                .Where(c => c.IsTeacher == true)
                .Select(d => new
                {
                    Duration = d.Courses.Select(e => (e.EndDate.Value - e.StartDate.Value).TotalHours).Sum(),
                    TeacherId = d.Id,
                    Fullname = d.FullName,
                    Price = d.Courses.Select(d => d.Price).Sum()
                }).ToList();
            return Ok(totalPrice);

            //var totalPrice = _dbContext.Courses
            //    .Where(c => c.TeacherId != null)
            //    .Select(c => new
            //    {
            //        Duration = (c.EndDate.Value - c.StartDate.Value).TotalDays,
            //        TeacherId = c.TeacherId,
            //        Fullname = c.Teacher.FullName,
            //        Price = c.Price
            //    })
            //    .GroupBy(c => c.TeacherId) //ex same id
            //    .Select(c => new
            //    {
            //        Time = c.Sum(c => c.Duration),
            //        Fullname = c.Select(c => c.Fullname).FirstOrDefault(),
            //        Price = c.Sum(c => c.Price)
            //    }).ToList();
            //return Ok(totalPrice);
        }
        [HttpGet("api/Courses/{coursename}")]
        public async Task<IActionResult> Getcoursbyname(string? name, DateTime? startDay, DateTime? endDay)
        {
            IQueryable<Course> allCourse = _dbContext.Courses;
            if (startDay != null)
            {
                allCourse = allCourse.Where(c => c.StartDate <= startDay);
            }
            if (endDay != null)
            {
                allCourse = allCourse.Where(c => c.EndDate >= endDay);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                allCourse = allCourse.Where(c => c.CourseName.Contains(name));
            }
            var matchingCourse = allCourse
                .Select(c => new
                {
                    CourseDetail = c,
                    TeacherName = c.Teacher.FullName,
                    Phone = c.Teacher.PhoneNumber,
                    author = _dbContext.AspNetUsers.Where(d => d.Id == c.CreatedUserId).Select(p => p.FullName).FirstOrDefault(),
                    updateby = _dbContext.AspNetUsers.Where(d => d.Id == c.UpdatedUserId).Select(p => p.FullName).FirstOrDefault()
                })
                .ToList();

            return Ok(matchingCourse);

            //var ontime = _dbContext.Courses
            //    .Where(c => c.StartDate >= startDay && c.EndDate <= endDay && c.CourseName.Contains(name))
            //    .Select(c => new 
            //    {
            //        name = c.Teacher.FullName,
            //        phone = c.Teacher.PhoneNumber,
            //        author = _dbContext.AspNetUsers.Where(e=>e.Id == c.CreatedUserId).Select(e=>e.FullName).FirstOrDefault(),//khong co thi tra ve null
            //        update = _dbContext.AspNetUsers.Where(e => e.Id == c.UpdatedUserId).Select(e => e.FullName).FirstOrDefault() // ten nguoi cap nhat
            //    })
            //    .ToList();

            //return Ok(ontime);
        }

        [HttpPost("api/Courses")]
        public async Task<ActionResult<IEnumerable<Course>>> PostCourse([FromBody] CourseReq course)
        {
            var courses = new Course
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Description = course.Description,
                TeacherId = course.TeacherId,
                Price = course.Price,
                MaxStudentQuantity = course.MaxStudentQuantity,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreateDate = DateTime.UtcNow
            };
            if (courses.StartDate > courses.EndDate)
            {
                return BadRequest("Start Date bigger End Date");
            }
            await _dbContext.Courses.AddAsync(courses);
            await _dbContext.SaveChangesAsync();
            return Ok(courses);
        }

        [HttpPut("api/Courses/{id}")]
        public async Task<ActionResult<IEnumerable<Course>>> PutCourse(string id, [FromBody] CourseReq course)
        {
            var courses = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == id);

            if (courses != null)
            {
                courses.CourseName = course.CourseName;
                courses.Description = course.Description;
                courses.TeacherId = course.TeacherId;
                courses.Price = course.Price;
                courses.MaxStudentQuantity = course.MaxStudentQuantity;
                courses.StartDate = course.StartDate;
                courses.EndDate = course.EndDate;
                courses.UpdatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                courses.UpdateDate = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("api/Courses/{id}/teacher")]
        public async Task<ActionResult<IEnumerable<Course>>> PutCourseTeacher(string id)
        {
            var courses = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == id);

            if (courses != null)
            {
                
                courses.TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("api/Courses/{id}")]
        public async Task<ActionResult<IEnumerable<Course>>> Delete(string id)
        {
            var course = await _dbContext.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            _dbContext.Courses.Remove(course);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
       
    }
}
