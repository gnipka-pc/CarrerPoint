namespace CareerPoint.Infrastructure.Model;

/// <summary>
/// Ответ студента на одно поле формы
/// </summary>
public class SubmissionAnswer
{
    public Guid Id { get; set; }

    public Guid SubmissionId { get; set; }

    public FormSubmission? Submission { get; set; }

    public Guid FieldId { get; set; }

    public FormField? Field { get; set; }

    /// <summary>
    /// Значение ответа. Для множественного выбора — значения через запятую.
    /// </summary>
    public required string Value { get; set; }
}
