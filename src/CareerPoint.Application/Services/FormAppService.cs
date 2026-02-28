using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.EntityFrameworkCore;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace CareerPoint.Application.Services;

public class FormAppService : IFormAppService
{
    private readonly CareerPointContext _context;

    public FormAppService(CareerPointContext context)
    {
        _context = context;
    }

    // ══════════════════════════════════════════════════════════════
    // МЕНЕДЖЕР
    // ══════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task<FormResponseDto> CreateFormAsync(CreateFormDto dto)
    {
        bool alreadyExists = await _context.Forms.AnyAsync(f => f.EventId == dto.EventId);
        if (alreadyExists)
            throw new InvalidOperationException($"Форма для мероприятия {dto.EventId} уже существует.");

        var form = new Form
        {
            Id = Guid.NewGuid(),
            EventId = dto.EventId,
            Title = dto.Title,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        form.Fields = MapFields(dto.Fields, form.Id);

        await _context.Forms.AddAsync(form);
        await _context.SaveChangesAsync();

        return MapToResponseDto(form);
    }

    /// <inheritdoc/>
    public async Task<FormResponseDto> UpdateFormAsync(Guid formId, UpdateFormDto dto)
    {
        Form form = await GetFormOrThrowAsync(formId);

        form.Title = dto.Title;
        form.Description = dto.Description;
        form.IsActive = dto.IsActive;
        form.UpdatedAt = DateTime.UtcNow;

        // Удаляем старые поля и заменяем новыми
        _context.FormFields.RemoveRange(form.Fields);
        form.Fields = MapFields(dto.Fields, form.Id);

        await _context.SaveChangesAsync();

        return MapToResponseDto(form);
    }

    /// <inheritdoc/>
    public async Task DeleteFormAsync(Guid formId)
    {
        Form form = await GetFormOrThrowAsync(formId);

        _context.Forms.Remove(form);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<FormResponseDto?> GetFormByEventIdAsync(Guid eventId)
    {
        Form? form = await _context.Forms
            .Include(f => f.Fields.OrderBy(ff => ff.Order))
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.EventId == eventId);

        return form is null ? null : MapToResponseDto(form);
    }

    /// <inheritdoc/>
    public async Task<FormResultsDto> GetFormResultsAsync(Guid formId)
    {
        Form form = await _context.Forms
            .Include(f => f.Fields)
            .Include(f => f.Submissions)
                .ThenInclude(s => s.Answers)
                    .ThenInclude(a => a.Field)
            .Include(f => f.Submissions)
                .ThenInclude(s => s.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == formId)
            ?? throw new KeyNotFoundException($"Форма {formId} не найдена.");

        return new FormResultsDto
        {
            FormId = form.Id,
            FormTitle = form.Title,
            TotalSubmissions = form.Submissions.Count,
            Submissions = form.Submissions
                .Select(s => MapSubmissionToDto(s))
                .ToList()
        };
    }

    // ══════════════════════════════════════════════════════════════
    // СТУДЕНТ
    // ══════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task<FormSubmissionResponseDto> SubmitFormAsync(
        Guid eventId,
        Guid studentId,
        SubmitFormDto dto)
    {
        Form form = await _context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.EventId == eventId)
            ?? throw new KeyNotFoundException($"Форма для мероприятия {eventId} не найдена.");

        if (!form.IsActive)
            throw new InvalidOperationException("Форма неактивна и не принимает ответы.");

        bool alreadySubmitted = await _context.FormSubmissions
            .AnyAsync(s => s.FormId == form.Id && s.StudentId == studentId);

        if (alreadySubmitted)
            throw new InvalidOperationException("Вы уже заполняли эту форму.");

        // Проверка обязательных полей
        var requiredFieldIds = form.Fields
            .Where(f => f.IsRequired)
            .Select(f => f.Id)
            .ToHashSet();

        var answeredFieldIds = dto.Answers.Select(a => a.FieldId).ToHashSet();
        var missingIds = requiredFieldIds.Except(answeredFieldIds).ToList();

        if (missingIds.Count > 0)
        {
            var missingKeys = form.Fields
                .Where(f => missingIds.Contains(f.Id))
                .Select(f => f.Key);

            throw new InvalidOperationException(
                $"Не заполнены обязательные поля: {string.Join(", ", missingKeys)}");
        }

        var validFieldIds = form.Fields.Select(f => f.Id).ToHashSet();

        var submission = new FormSubmission
        {
            Id = Guid.NewGuid(),
            FormId = form.Id,
            StudentId = studentId,
            SubmittedAt = DateTime.UtcNow,
            Answers = dto.Answers
                .Where(a => validFieldIds.Contains(a.FieldId))
                .Select(a => new SubmissionAnswer
                {
                    Id = Guid.NewGuid(),
                    FieldId = a.FieldId,
                    Value = a.Value
                })
                .ToList()
        };

        await _context.FormSubmissions.AddAsync(submission);
        await _context.SaveChangesAsync();

        // Перезагружаем с навигационными свойствами для ответа
        submission = await _context.FormSubmissions
            .Include(s => s.Answers).ThenInclude(a => a.Field)
            .Include(s => s.Student)
            .FirstAsync(s => s.Id == submission.Id);

        return MapSubmissionToDto(submission);
    }

    /// <inheritdoc/>
    public async Task<FormWithMyAnswersDto?> GetMySubmissionByEventIdAsync(Guid eventId, Guid studentId)
    {
        Form? form = await _context.Forms
            .Include(f => f.Fields)
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.EventId == eventId);

        if (form is null)
            return null;

        FormSubmission? submission = await _context.FormSubmissions
            .Include(s => s.Answers).ThenInclude(a => a.Field)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.FormId == form.Id && s.StudentId == studentId);

        if (submission is null)
            return null;

        var fieldMap = form.Fields.ToDictionary(f => f.Id);

        return new FormWithMyAnswersDto
        {
            FormId = form.Id,
            EventId = form.EventId,
            FormTitle = form.Title,
            SubmissionId = submission.Id,
            SubmittedAt = submission.SubmittedAt,
            Fields = submission.Answers
                .Where(a => fieldMap.ContainsKey(a.FieldId))
                .Select(a => new FilledFieldDto
                {
                    FieldId = a.FieldId,
                    Key = fieldMap[a.FieldId].Key,
                    Label = fieldMap[a.FieldId].Label,
                    Value = a.Value
                })
                .ToList()
        };
    }

    // ══════════════════════════════════════════════════════════════
    // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
    // ══════════════════════════════════════════════════════════════

    private async Task<Form> GetFormOrThrowAsync(Guid formId)
    {
        return await _context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Id == formId)
            ?? throw new KeyNotFoundException($"Форма {formId} не найдена.");
    }

    private static List<FormField> MapFields(List<FormFieldDto> dtos, Guid formId)
    {
        return dtos.Select(dto => new FormField
        {
            Id = Guid.NewGuid(),
            FormId = formId,
            Key = dto.Key,
            Label = dto.Label,
            Placeholder = dto.Placeholder,
            Type = dto.Type,
            IsRequired = dto.IsRequired,
            Order = dto.Order,
            Options = dto.Options is { Count: > 0 }
                ? string.Join(",", dto.Options)
                : null
        }).ToList();
    }

    private static FormResponseDto MapToResponseDto(Form form)
    {
        return new FormResponseDto
        {
            Id = form.Id,
            EventId = form.EventId,
            Title = form.Title,
            Description = form.Description,
            IsActive = form.IsActive,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt,
            Fields = form.Fields
                .OrderBy(f => f.Order)
                .Select(f => new FormFieldResponseDto
                {
                    Id = f.Id,
                    Key = f.Key,
                    Label = f.Label,
                    Placeholder = f.Placeholder,
                    Type = f.Type,
                    IsRequired = f.IsRequired,
                    Order = f.Order,
                    Options = f.Options is not null
                        ? f.Options.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                        : null
                })
                .ToList()
        };
    }

    private static FormSubmissionResponseDto MapSubmissionToDto(FormSubmission submission)
    {
        return new FormSubmissionResponseDto
        {
            Id = submission.Id,
            FormId = submission.FormId,
            StudentId = submission.StudentId,
            StudentName = submission.Student is not null
                ? $"{submission.Student.Name} {submission.Student.Surname}"
                : null,
            SubmittedAt = submission.SubmittedAt,
            Answers = submission.Answers.Select(a => new AnswerResponseDto
            {
                FieldId = a.FieldId,
                FieldKey = a.Field?.Key ?? string.Empty,
                FieldLabel = a.Field?.Label ?? string.Empty,
                Value = a.Value
            }).ToList()
        };
    }
}
