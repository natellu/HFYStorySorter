using HFYStorySorter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Controllers.Api
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new { user.Id, user.Email, user.IsAdmin });
        }


        //todo protect for admin users only
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.Select(u => new { u.Id, u.Email, u.IsAdmin }).ToListAsync();
            return Ok(users);
        }


        //todo login, register, confirmemail, edit profile, etc
    }
}
