namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Результат прохождения формы конкретным студентом
/// </summary>
public class FormSubmission
{
    public Guid Id { get; set; }

    public Guid FormId { get; set; }

    public Form? Form { get; set; }

    /// <summary>ID студента (User.Id)</summary>
    public Guid StudentId { get; set; }

    public User? Student { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public List<SubmissionAnswer> Answers { get; set; } = new();
}
