using Microsoft.EntityFrameworkCore;
using Lernzeit.Domain;

namespace Lernzeit.PostgresAdapter;

public class LernzeitDbContext : DbContext
{
    public LernzeitDbContext(DbContextOptions<LernzeitDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroups> UserGroups { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroups>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });
            modelBuilder.Entity<UserGroups>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);
            modelBuilder.Entity<UserGroups>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId);
        }
}