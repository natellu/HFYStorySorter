
using HFYStorySorter.Data;
using HFYStorySorter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Controllers.Api
{
    [ApiController]
    [Route("api/stories")]
    public class StoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStories()
        {
            var stories = await _db.Stories
                .Select(s => new { s.Id, s.StoryName })
                .ToListAsync();

            return Ok(stories);
        }

        [HttpPost("{id}/subscribe")]
        public async Task<IActionResult> Subscribe(Guid id, [FromQuery] Guid userId)
        {
            var exists = await _db.SubscribedStories.AnyAsync(x => x.UserId == userId && x.StoryId == id);
            if (exists) return Conflict("Already subscribed.");

            _db.SubscribedStories.Add(new UserStorySubscription { UserId = userId, StoryId = id });
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
