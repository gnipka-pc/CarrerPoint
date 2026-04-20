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

    public FormController(IFormAppService formAppService)
    {
        _formAppService = formAppService;
    }
    
    /// <summary>
    /// Создание формы регистрации для мероприятия
    /// </summary>
    [Authorize(Roles = "Manager,Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateFormAsync([FromBody] CreateFormDto dto)
    {
        try
        {
            FormResponseDto result = await _formAppService.CreateFormAsync(dto);
            return Ok(result.Id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Обновление формы регистрации
    /// </summary>
    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("{formId:guid}")]
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
    [Authorize(Roles = "Manager,Admin")]
    [HttpDelete("{formId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteFormAsync(Guid formId)
    {
        try
        {
            await _formAppService.DeleteFormAsync(formId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Получение формы по ID мероприятия (менеджер)
    /// </summary>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("by-event/{eventId:guid}")]
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
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("{formId:guid}/results")]
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
    
    /// <summary>
    /// Получение формы регистрации по ID мероприятия (студент)
    /// </summary>
    [Authorize(Roles = "DefaultUser")]
    [HttpGet("event/{eventId:guid}")]
    [ProducesResponseType(typeof(FormResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFormByEventIdForStudentAsync(Guid eventId)
    {
        FormResponseDto? form = await _formAppService.GetFormByEventIdAsync(eventId);

        if (form is null)
            return NotFound($"Форма для мероприятия {eventId} не найдена.");

        if (!form.IsOpen)
            return BadRequest("Форма закрыта.");

        return Ok(form);
    }

    /// <summary>
    /// Прохождение формы регистрации студентом
    /// </summary>
    [Authorize(Roles = "DefaultUser")]
    [HttpPost("event/{eventId:guid}/submit")]
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
    [Authorize(Roles = "DefaultUser")]
    [HttpGet("event/{eventId:guid}/my-submission")]
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

    private Guid? GetCurrentUserId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid id) ? id : null;
    }
}
