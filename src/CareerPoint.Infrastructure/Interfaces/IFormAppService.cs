using CareerPoint.Infrastructure.DTOs;

namespace CareerPoint.Infrastructure.Interfaces;

public interface IFormAppService
{
    //Менеджер

    /// <summary>Создать форму для мероприятия</summary>
    Task<FormResponseDto> CreateFormAsync(CreateFormDto dto);

    /// <summary>Обновить форму (заголовок, поля, статус активности)</summary>
    Task<FormResponseDto> UpdateFormAsync(Guid formId, UpdateFormDto dto);

    /// <summary>Удалить форму и все связанные данные</summary>
    Task DeleteFormAsync(Guid formId);

    /// <summary>Получить форму по ID мероприятия</summary>
    Task<FormResponseDto?> GetFormByEventIdAsync(Guid eventId);

    /// <summary>Получить все ответы на форму по её ID (результаты)</summary>
    Task<FormResultsDto> GetFormResultsAsync(Guid formId);

    //Студент

    /// <summary>Заполнить форму по ID мероприятия</summary>
    Task<FormSubmissionResponseDto> SubmitFormAsync(Guid eventId, Guid studentId, SubmitFormDto dto);

    /// <summary>Получить заполненную форму студента по ID мероприятия</summary>
    Task<FormWithMyAnswersDto?> GetMySubmissionByEventIdAsync(Guid eventId, Guid studentId);
}
