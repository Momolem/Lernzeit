using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Lernzeit.PostgresAdapter.Entities;

namespace Lernzeit.PostgresAdapter;

public class LernzeitDbContext : DbContext
{
    public LernzeitDbContext(DbContextOptions<LernzeitDbContext> options) : base(options) { }

        public DbSet<RaumzeitToken> RaumzeitTokens { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupEntity>()
                .HasMany(g => g.Members)
                .WithMany(u => u.Groups)
                .UsingEntity("UserGroups");
        }
}

public class RaumzeitToken
{
    [Key]
    public required Guid UserId { get; set; }
    
    public required string EncryptedToken { get; set; }
    
    public DateTimeOffset Expiration { get; set; }
    
}