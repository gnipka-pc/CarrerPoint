using CareerPoint.Infrastructure.Enums;

namespace CareerPoint.Infrastructure.DTOs;

//Запросы (входящие данные)
public class CreateFormDto
{
    public Guid EventId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public List<FormFieldDto> Fields { get; set; } = new();
}

public class UpdateFormDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public List<FormFieldDto> Fields { get; set; } = new();
}

public class FormFieldDto
{
    public required string Key { get; set; }

    public required string Label { get; set; }

    public string? Placeholder { get; set; }

    public FieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public int Order { get; set; }

    /// <summary>Варианты для Select / Radio / Checkbox</summary>
    public List<string>? Options { get; set; }
}

public class SubmitFormDto
{
    public List<AnswerDto> Answers { get; set; } = new();
}

public class AnswerDto
{
    public Guid FieldId { get; set; }

    public required string Value { get; set; }
}

//Ответы (исходящие данные)
public class FormResponseDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<FormFieldResponseDto> Fields { get; set; } = new();
}

public class FormFieldResponseDto
{
    public Guid Id { get; set; }
    public required string Key { get; set; }
    public required string Label { get; set; }
    public string? Placeholder { get; set; }
    public FieldType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public List<string>? Options { get; set; }
}

public class FormSubmissionResponseDto
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<AnswerResponseDto> Answers { get; set; } = new();
}

public class AnswerResponseDto
{
    public Guid FieldId { get; set; }
    public required string FieldKey { get; set; }
    public required string FieldLabel { get; set; }
    public required string Value { get; set; }
}

public class FormResultsDto
{
    public Guid FormId { get; set; }
    public required string FormTitle { get; set; }
    public int TotalSubmissions { get; set; }
    public List<FormSubmissionResponseDto> Submissions { get; set; } = new();
}

/// <summary>Форма с заполненными ответами для студента</summary>
public class FormWithMyAnswersDto
{
    public Guid FormId { get; set; }
    public Guid EventId { get; set; }
    public required string FormTitle { get; set; }
    public Guid SubmissionId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<FilledFieldDto> Fields { get; set; } = new();
}

public class FilledFieldDto
{
    public Guid FieldId { get; set; }
    public required string Key { get; set; }
    public required string Label { get; set; }
    public required string Value { get; set; }
}
