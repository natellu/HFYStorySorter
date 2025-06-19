using HFYStorySorter.Models;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Story> Stories => Set<Story>();
        public DbSet<Chapter> Chapters => Set<Chapter>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
