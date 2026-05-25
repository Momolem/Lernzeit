using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter;

public class LernzeitDbContext : DbContext
{
    public DbSet<RaumzeitToken> RaumzeitTokens { get; set; }
    
    protected LernzeitDbContext()
    {
    }

    public LernzeitDbContext(DbContextOptions options) : base(options)
    {
    }
}

public class RaumzeitToken
{
    [Key]
    public required string UserId { get; set; }
    
    public required string EncryptedToken { get; set; }
    
    public DateTimeOffset Expiration { get; set; }
    
}