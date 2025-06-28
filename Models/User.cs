using System.ComponentModel.DataAnnotations;

namespace HFYStorySorter.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; } = false;

        public bool IsAdmin { get; set; } = false;

        public List<UserStorySubscription> SubscribedStories { get; set; } = new();
        public List<UserChapterRead> ReadChapters { get; set; } = new();
    }
}
