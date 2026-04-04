using CareerPoint.Infrastructure.DTOs;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IFormAppService
{
    // ── Менеджер ──────────────────────────────────────────────────

    /// <summary>Создать форму регистрации для мероприятия</summary>
    Task<FormResponseDto> CreateFormAsync(CreateFormDto dto);

    /// <summary>Обновить форму регистрации</summary>
    Task<FormResponseDto> UpdateFormAsync(Guid formId, UpdateFormDto dto);

    /// <summary>Удалить форму регистрации</summary>
    Task DeleteFormAsync(Guid formId);

    /// <summary>Получить форму по ID мероприятия</summary>
    Task<FormResponseDto?> GetFormByEventIdAsync(Guid eventId);

    /// <summary>Получить все результаты прохождения формы</summary>
    Task<FormResultsDto> GetFormResultsAsync(Guid formId);

    // ── Студент ───────────────────────────────────────────────────

    /// <summary>Отправить заполненную форму</summary>
    Task<FormSubmissionResponseDto> SubmitFormAsync(Guid eventId, Guid studentId, SubmitFormDto dto);

    /// <summary>Получить форму с ответами студента по ID мероприятия</summary>
    Task<FormWithMyAnswersDto?> GetMySubmissionByEventIdAsync(Guid eventId, Guid studentId);
}
