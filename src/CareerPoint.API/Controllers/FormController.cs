using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPoint.Web.Controllers;

/// <summary>
/// Контроллер форм регистрации на мероприятия
/// </summary>
[Route("api/forms")]
[ApiController]
public class FormController : ControllerBase
{
    private readonly IFormAppService _formAppService;

    /// <summary>
    /// Базовый конструктор контроллера форм
    /// </summary>
    public FormController(IFormAppService formAppService)
    {
        _formAppService = formAppService;
    }


    //МЕНЕДЖЕР

    /// <summary>
    /// Создание формы регистрации для мероприятия
    /// </summary>
    /// <param name="dto">Данные формы</param>
    /// <returns>Созданная форма</returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpPost("create")]
    [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateFormAsync([FromBody] CreateFormDto dto)
    {
        try
        {
            FormResponseDto result = await _formAppService.CreateFormAsync(dto);
            return CreatedAtAction(nameof(GetFormByEventIdForManagerAsync),
                new { eventId = result.EventId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Обновление формы регистрации
    /// </summary>
    /// <param name="formId">ID формы</param>
    /// <param name="dto">Новые данные формы</param>
    /// <returns>Обновлённая форма</returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("update/{formId}")]
    [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateFormAsync(Guid formId, [FromBody] UpdateFormDto dto)
    {
        try
        {
            FormResponseDto result = await _formAppService.UpdateFormAsync(formId, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Удаление формы регистрации
    /// </summary>
    /// <param name="formId">ID формы</param>
    [Authorize(Roles = "Manager,Admin")]
    [HttpDelete("delete/{formId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteFormAsync(Guid formId)
    {
        try
        {
            await _formAppService.DeleteFormAsync(formId);
            return Ok("Форма удалена успешно.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Получение формы по ID мероприятия (менеджер)
    /// </summary>
    /// <param name="eventId">ID мероприятия</param>
    /// <returns>Форма регистрации</returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("by-event/{eventId}")]
    [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFormByEventIdForManagerAsync(Guid eventId)
    {
        FormResponseDto? form = await _formAppService.GetFormByEventIdAsync(eventId);

        if (form is null)
            return NotFound($"Форма для мероприятия {eventId} не найдена.");

        return Ok(form);
    }

    /// <summary>
    /// Получение результатов прохождения формы (все ответы студентов)
    /// </summary>
    /// <param name="formId">ID формы</param>
    /// <returns>Результаты с ответами всех студентов</returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("{formId}/results")]
    [ProducesResponseType(typeof(FormResultsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFormResultsAsync(Guid formId)
    {
        try
        {
            FormResultsDto results = await _formAppService.GetFormResultsAsync(formId);
            return Ok(results);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    //СТУДЕНТ

    /// <summary>
    /// Получение формы регистрации по ID мероприятия (студент)
    /// </summary>
    /// <param name="eventId">ID мероприятия</param>
    /// <returns>Форма с полями для заполнения</returns>
    [Authorize(Roles = "DefaultUser")]
    [HttpGet("event/{eventId}")]
    [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFormByEventIdForStudentAsync(Guid eventId)
    {
        FormResponseDto? form = await _formAppService.GetFormByEventIdAsync(eventId);

        if (form is null)
            return NotFound($"Форма для мероприятия {eventId} не найдена.");

        if (!form.IsActive)
            return BadRequest("Форма неактивна.");

        return Ok(form);
    }

    /// <summary>
    /// Прохождение формы регистрации студентом
    /// </summary>
    /// <param name="eventId">ID мероприятия</param>
    /// <param name="dto">Ответы на поля формы</param>
    /// <returns>Сохранённые ответы</returns>
    [Authorize(Roles = "DefaultUser")]
    [HttpPost("event/{eventId}/submit")]
    [ProducesResponseType(typeof(FormSubmissionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubmitFormAsync(Guid eventId, [FromBody] SubmitFormDto dto)
    {
        Guid? studentId = GetCurrentUserId();
        if (studentId is null)
            return Unauthorized();

        try
        {
            FormSubmissionResponseDto result =
                await _formAppService.SubmitFormAsync(eventId, studentId.Value, dto);

            return CreatedAtAction(nameof(GetMySubmissionByEventIdAsync),
                new { eventId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Получение данных формы, которую студент заполнял, по ID мероприятия
    /// </summary>
    /// <param name="eventId">ID мероприятия</param>
    /// <returns>Форма с ответами студента</returns>
    [Authorize(Roles = "DefaultUser")]
    [HttpGet("event/{eventId}/my-submission")]
    [ProducesResponseType(typeof(FormWithMyAnswersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMySubmissionByEventIdAsync(Guid eventId)
    {
        Guid? studentId = GetCurrentUserId();
        if (studentId is null)
            return Unauthorized();

        FormWithMyAnswersDto? result =
            await _formAppService.GetMySubmissionByEventIdAsync(eventId, studentId.Value);

        if (result is null)
            return NotFound("Вы не заполняли форму для этого мероприятия.");

        return Ok(result);
    }

    //Вспомогательные методы

    private Guid? GetCurrentUserId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid id) ? id : null;
    }
}
