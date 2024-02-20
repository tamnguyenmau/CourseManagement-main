using linq.model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace linq.Controllers
{
    public class RoleController : Controller
    {

        private readonly PostgresContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager, PostgresContext dbContext)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
        }
        [HttpGet("api/Role")]
        public async Task<ActionResult<IEnumerable<AspNetRole>>> GetRoles()
        {
            var roles = await _dbContext.AspNetRoles.ToListAsync();
            return Ok(roles);
        }

        //[HttpPost("Role/create-role-for-user")]
        //public async Task<IActionResult> Create(AspNetRole model)
        //{
           
        //    var roles = new AspNetRole
        //    {
        //        Name = model.Name

        //    };
        //    await _dbContext.AspNetRoles.AddAsync(roles);
        //    await _dbContext.SaveChangesAsync();
        //    return Ok(roles);

        //    //var check = _dbContext.AspNetRoles.FirstOrDefault(x => x.Name == model.Name);
        //    //if (check == null)
        //    //{
                
        //    //}
        //    //return BadRequest();
        //}
    }
}
