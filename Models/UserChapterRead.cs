namespace HFYStorySorter.Models
{
    public class UserChapterRead
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid ChapterId { get; set; }
        public Chapter Chapter { get; set; } = null!;
    }
}
