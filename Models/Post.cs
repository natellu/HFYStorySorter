using System.ComponentModel.DataAnnotations;

namespace HFYStorySorter.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RedditId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }
        public bool IsProcessed { get; set; } = false;
    }
}
