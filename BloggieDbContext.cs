using Microsoft.EntityFrameworkCore;

class BloggieDbContext : DbContext
{
    public BloggieDbContext(DbContextOptions<BloggieDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
}

