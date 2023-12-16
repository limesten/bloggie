using Microsoft.EntityFrameworkCore;

class BloggieDbContext : DbContext
{
    public BloggieDbContext(DbContextOptions<BloggieDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var e in entries)
        {
            ((BaseEntity)e.Entity).UpdatedAt = DateTime.UtcNow;

            if (e.State == EntityState.Added)
            {
                ((BaseEntity)e.Entity).CreatedAt = DateTime.UtcNow;
            }

            Console.WriteLine(((BaseEntity)e.Entity).CreatedAt);
            Console.WriteLine(((BaseEntity)e.Entity).UpdatedAt);
        }
        return await base.SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var e in entries)
        {
            ((BaseEntity)e.Entity).UpdatedAt = DateTime.UtcNow;

            if (e.State == EntityState.Added)
            {
                ((BaseEntity)e.Entity).CreatedAt = DateTime.UtcNow;
            }

            Console.WriteLine(((BaseEntity)e.Entity).CreatedAt);
            Console.WriteLine(((BaseEntity)e.Entity).UpdatedAt);
        }
        return base.SaveChanges();
    }
}

