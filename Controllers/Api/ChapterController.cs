using HFYStorySorter.Data;
using HFYStorySorter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Controllers.Api
{
    [ApiController]
    [Route("api/chapters")]
    public class ChapterController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ChapterController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{storyId}")]
        public async Task<IActionResult> GetChaptersForStory(Guid storyId)
        {
            var chapters = await _db.Chapters
                .Where(c => c.StoryId == storyId)
                .Select(c => new { c.Id, c.ChapterNumber, c.Post.Title })
                .ToListAsync();

            return Ok(chapters);
        }

        [HttpPost("{chapterId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid chapterId, [FromQuery] Guid userId)
        {
            var already = await _db.ReadChapters.AnyAsync(x => x.UserId == userId && x.ChapterId == chapterId);
            if (already) return Conflict("Already marked as read.");

            _db.ReadChapters.Add(new UserChapterRead { UserId = userId, ChapterId = chapterId });
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
