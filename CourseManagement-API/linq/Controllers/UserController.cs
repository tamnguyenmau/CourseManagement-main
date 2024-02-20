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
    public class UserController : Controller
    {
        private readonly PostgresContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        public UserController(PostgresContext dbContext, UserManager<AspNetUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }

        [HttpGet("api/Users/{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [Authorize]
        [HttpGet("api/Users/login-information")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("api/Users")]
        public async Task<IActionResult> getAll()
        {
            var user = _dbContext.AspNetUsers.ToList();
            return Ok(user);
        }
        [Authorize(Roles = "Manager")]
        [HttpGet("api/Users/teacher")]
        public async Task<IActionResult> Getteacher()
        {
            var teacher = _dbContext.AspNetUsers
                .Where(c => c.IsTeacher == true)
                .ToList();

            return Ok(teacher);
        }
        [Authorize(Roles = "Manager")]
        [HttpGet("api/Users/student")]
        public async Task<IActionResult> Getstudent()
        {
            var student = _dbContext.AspNetUsers
                .Where(c => c.IsStudent == true)
                .ToList();

            return Ok(student);
        }
        [Authorize(Roles = "Manager")]
        [HttpPost("api/Users/teacher")]
        public async Task<ActionResult<IEnumerable<Course>>> PostTeacher([FromBody] TeacherReq request)
        {
            var teachers = new AspNetUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Address = request.Address,
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = request.EmailConfirmed == true,
                IsTeacher = request.IsTeacher = true,
                LockDate = request.LockDate = DateTime.Now.AddYears(1).ToUniversalTime(),
                LockoutEnabled = true

            };
            var result = await _userManager.CreateAsync(teachers, request.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }
        [Authorize(Roles = "Manager")]
        [HttpPost("api/Users/student")]
        public async Task<ActionResult<IEnumerable<Course>>> PostStudent([FromBody] StudentReq request)
        {
            var teachers = new AspNetUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Address = request.Address,
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = request.EmailConfirmed == true,
                IsStudent = request.IsStudent = true,
                LockDate = request.LockDate = DateTime.Now.AddYears(1).ToUniversalTime(),
                LockoutEnabled = true

            };
            var result = await _userManager.CreateAsync(teachers, request.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPut("api/Users/{id}")]
        public async Task<ActionResult<IEnumerable<AspNetUser>>> Put(string id, [FromBody] UserReq user)
        {

            var users = await _dbContext.AspNetUsers.FirstOrDefaultAsync(d => d.Id == id);
            if (users != null)
            {   
                users.FullName = user.FullName;
                users.Address = user.Address;
                users.UserName = user.UserName;
                users.Email = user.Email;
                users.LockDate = user.LockDate;
                users.LockoutEnabled = user.LockoutEnabled;
                users.Image = user.Image;
            }

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("api/Users/{id}")]
        public async Task<ActionResult<IEnumerable<AspNetUser>>> Delete(string id)
        {
            var users = await _dbContext.AspNetUsers.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            _dbContext.AspNetUsers.Remove(users);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
    }
}
