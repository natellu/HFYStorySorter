using HFYStorySorter.Data;
using HFYStorySorter.Helpers;
using HFYStorySorter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Controllers.Api
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public UserController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
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


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.Select(u => new { u.Id, u.Email, u.IsAdmin }).ToListAsync();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing != null) return Conflict("Email already registered.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = AuthHelper.HashPassword(dto.Password),
                IsEmailConfirmed = false,
                IsAdmin = false
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            //todo confirmation email

            return Ok(new { user.Id, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !AuthHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            if (!user.IsEmailConfirmed) return Unauthorized("Email not confirmed");

            var token = AuthHelper.GenerateJwt(user, _config["Jwt:Secret"]);
            return Ok(new { token });

        }


        //todo confirm email, edit profile, etc
    }
}
