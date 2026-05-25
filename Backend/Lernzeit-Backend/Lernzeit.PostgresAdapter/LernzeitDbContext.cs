using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter;

public class LernzeitDbContext : DbContext
{
    protected LernzeitDbContext()
    {
    }

    public LernzeitDbContext(DbContextOptions options) : base(options)
    {
    }
}