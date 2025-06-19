using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HFYStorySorter.Models
{
    public class Chapter
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Story")]
        public Guid StoryId { get; set; }
        public Story? Story { get; set; }

        [ForeignKey("Post")]
        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public int ChapterNumber { get; set; }
    }
}
