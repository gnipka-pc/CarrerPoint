using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Infrastructure.EntityFrameworkCore;

public class CareerPointContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Event> Events { get; set; }

    //public CareerPointContext(DbContextOptions<CareerPointContext> options) : base(options)
    //{
    //}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;user=root;password=CyEnREzXX12b;database=CareerPoint",
            new MySqlServerVersion(new Version(8, 0, 41)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasMany(u => u.Events).WithMany(e => e.Users);
    }
}
