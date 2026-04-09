using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Infrastructure.EntityFrameworkCore;

public class CareerPointContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Event> Events { get; set; }
    
    public DbSet<Form> Forms { get; set; }
    
    public DbSet<FormField> FormFields { get; set; }
    
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    
    public DbSet<SubmissionAnswer> SubmissionAnswers { get; set; }

    public DbSet<EventFavorite> EventFavorites { get; set; }

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
            .Property(e => e.HardSkills)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        modelBuilder.Entity<EventFavorite>()
            .HasKey(x => new { x.UserId, x.EventId });

        modelBuilder.Entity<EventFavorite>()
            .Property(x => x.CreatedAt);

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
        
        modelBuilder.Entity<Form>(b =>
        {
            b.HasKey(f => f.Id);

            // Одна форма на одно мероприятие
            b.HasIndex(f => f.EventId).IsUnique();

            b.HasOne(f => f.Event)
                .WithMany()
                .HasForeignKey(f => f.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(f => f.Fields)
                .WithOne(ff => ff.Form)
                .HasForeignKey(ff => ff.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(f => f.Submissions)
                .WithOne(s => s.Form)
                .HasForeignKey(s => s.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(f => f.IsOpen).HasDefaultValue(true);
            b.Property(f => f.DeadlineAt).IsRequired(false);
        });

        modelBuilder.Entity<FormField>(b =>
        {
            b.HasKey(ff => ff.Id);

            // Тип поля хранится как строка
            b.Property(ff => ff.Type).HasConversion<string>();

            b.HasMany(ff => ff.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionOption>(b =>
        {
            b.HasKey(o => o.Id);

            // Value необязателен; если не указан — при сравнении ответов используется Text
            b.Property(o => o.Value).IsRequired(false);
        });

        modelBuilder.Entity<FormSubmission>(b =>
        {
            b.HasKey(s => s.Id);

            // Студент может заполнить форму только один раз
            b.HasIndex(s => new { s.FormId, s.StudentId }).IsUnique();

            b.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(s => s.Answers)
                .WithOne(a => a.Submission)
                .HasForeignKey(a => a.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubmissionAnswer>(b =>
        {
            b.HasKey(a => a.Id);

            b.HasOne(a => a.Field)
                .WithMany()
                .HasForeignKey(a => a.FieldId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
