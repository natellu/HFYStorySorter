using System.ComponentModel.DataAnnotations;

namespace HFYStorySorter.Models
{
    public class Story
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string StoryName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }

        public List<Chapter> Chapters { get; set; } = new();
    }
}
