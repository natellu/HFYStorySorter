namespace HFYStorySorter.Models
{
    public class UserStorySubscription
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid StoryId { get; set; }
        public Story Story { get; set; } = null!;
    }
}
