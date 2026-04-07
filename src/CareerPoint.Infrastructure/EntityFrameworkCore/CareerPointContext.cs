using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Infrastructure.EntityFrameworkCore;

public class CareerPointContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<EventTag> EventTags { get; set; }

    public CareerPointContext(DbContextOptions<CareerPointContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasMany(u => u.Events).WithMany(e => e.Users);

        modelBuilder.Entity<User>()
            .Property(u => u.Skills)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        modelBuilder.Entity<Event>()
            .HasMany(e => e.Tags)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventTag>()
            .HasIndex(t => new { t.EventId, t.Key });
    }
}
