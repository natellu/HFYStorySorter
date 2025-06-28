using HFYStorySorter.Models;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Story> Stories => Set<Story>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserStorySubscription> SubscribedStories => Set<UserStorySubscription>();
        public DbSet<UserChapterRead> ReadChapters => Set<UserChapterRead>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserStorySubscription>()
                .HasKey(x => new { x.UserId, x.StoryId });

            modelBuilder.Entity<UserStorySubscription>()
                .HasOne(x => x.User)
                .WithMany(u => u.SubscribedStories)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserStorySubscription>()
                .HasOne(x => x.Story)
                .WithMany()
                .HasForeignKey(x => x.StoryId);

            modelBuilder.Entity<UserChapterRead>()
                .HasKey(x => new { x.UserId, x.ChapterId });

            modelBuilder.Entity<UserChapterRead>()
                .HasOne(x => x.User)
                .WithMany(u => u.ReadChapters)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserChapterRead>()
                .HasOne(x => x.Chapter)
                .WithMany()
                .HasForeignKey(x => x.ChapterId);
        }
    }
}
