using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Infrastructure.EntityFrameworkCore;

public class CareerPointContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<EventFavorite> EventFavorites { get; set; }

    //public CareerPointContext(DbContextOptions<CareerPointContext> options) : base(options)
    //{
    //}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "server=localhost;port=3306;user=root;password=root;database=CareerPoint;AllowPublicKeyRetrieval=True;SslMode=None;";

        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasMany(u => u.Events).WithMany(e => e.Users);

        modelBuilder.Entity<EventFavorite>()
            .HasKey(x => new { x.UserId, x.EventId });

        modelBuilder.Entity<EventFavorite>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("UTC_TIMESTAMP()");

        modelBuilder.Entity<EventFavorite>()
            .HasOne(x => x.User)
            .WithMany(x => x.EventFavorites)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventFavorite>()
            .HasOne(x => x.Event)
            .WithMany(x => x.EventFavorites)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventFavorite>()
            .HasIndex(x => new { x.UserId, x.CreatedAt });

        modelBuilder.Entity<EventFavorite>()
            .HasIndex(x => x.EventId);

        modelBuilder.Entity<User>()
            .Property(u => u.Skills)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }
}
