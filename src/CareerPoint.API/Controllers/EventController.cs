using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Interfaces;
using CareerPoint.Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace CareerPoint.Web.Controllers;

/// <summary>
/// Контроллер ивентов
/// </summary>
[Route("api/events")]
[ApiController]
public class EventController : ControllerBase
{
    readonly IEventAppService _eventAppService;
    readonly IMinioClient _minioClient;

    /// <summary>
    /// Базовый конструктор контроллера ивентов
    /// </summary>
    /// <param name="eventAppService">Апп сервис ивентов</param>
    /// <param name="minioClient">Minio клиент</param>
    public EventController(IEventAppService eventAppService, IMinioClient minioClient)
    {
        _eventAppService = eventAppService;
        _minioClient = minioClient;
    }

    /// <summary>
    /// Получение ивента по его айди
    /// </summary>
    /// <param name="id">Айди ивента</param>
    /// <returns>Ивент</returns>
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpGet("get-event/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventByIdAsync(Guid id)
    {
        EventDto? ev = await _eventAppService.GetEventByIdAsync(id);

        if (ev != null)
            return Ok(ev);

        return NotFound("Ивент не найден");
    }

    /// <summary>
    /// Получение списка ивентов
    /// </summary>
    /// <returns>Список ивентов</returns>
    [Authorize(Roles = "DefaultUser,Manager,Admin")]
    [HttpGet("get-events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsAsync()
    {
        return Ok(await _eventAppService.GetEventsAsync());
    }

    /// <summary>
    /// Создание ивента
    /// </summary>
    /// <param name="createDto">Данные для создания ивента</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager")]
    [HttpPost("create-event")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateEventAsync([FromBody] CreateEventDto createDto)
    {
        await _eventAppService.CreateEventAsync(createDto);
        return Ok("Ивент создан успешно");
    }

    /// <summary>
    /// Удаление ивента по его айди
    /// </summary>
    /// <param name="id">Айди ивента</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager")]
    [HttpDelete("delete-event/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEventAsync(Guid id)
    {
        EventDto? ev = await _eventAppService.GetEventByIdAsync(id);

        if (ev != null)
        {
            await _eventAppService.DeleteEventAsync(id);
            return Ok("Ивент удален успешно");
        }

        return NotFound("Ивент с данным id не был найден");
    }

    /// <summary>
    /// Обновление ивента
    /// </summary>
    /// <param name="id">ID ивента</param>
    /// <param name="eventDto">Данные ивента</param>
    /// <returns></returns>
    [Authorize(Roles = "Manager")]
    [HttpPut("update-event/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEventAsync(Guid id, [FromBody] EventDto eventDto)
    {
        if (await _eventAppService.GetEventByIdAsync(id) != null)
        {
            await _eventAppService.UpdateEventAsync(id, eventDto);
            return Ok("Ивент изменен успешно");
        }

        return NotFound("Ивент с данным id не был найден");
    }


    /// <summary>
    /// Получение событий пользователя по ID (для менеджера)
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="userAppService">Сервис пользователей</param>
    /// <param name="mapper">Маппер</param>
    /// <returns>Список событий пользователя</returns>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("get-user-events/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserEventsForManagerAsync(
        Guid userId,
        [FromServices] IUserAppService userAppService,
        [FromServices] IMapper mapper)
    {
        List<Event> events = await userAppService.GetUserEventsAsync(userId);

        if (events.Count == 0)
            return NotFound("У пользователя нет ивентов");

        return Ok(mapper.Map<List<EventDto>>(events));
    }

    /// <summary>
    /// Добавление картинки к событию по индексу от 1 до 4 включительно
    /// </summary>
    /// <param name="eventId">Айди события</param>
    /// <param name="index">Индекс</param>
    /// <param name="file">Картинка</param>
    /// <returns>Код статуса ответа</returns>
    [HttpPost("add-image/{eventId:guid}/{index:int}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddImageToEvent(Guid eventId, int index, IFormFile file)
    {
        if (await _eventAppService.GetEventByIdAsync(eventId) == null)
            return BadRequest("Ивента с таким айди не существует");

        if (file == null || file.Length == 0)
            return BadRequest("Файл не передан");

        if (file.Length > 5 * 1024 * 1024) // 5 MB
            return BadRequest("Слишком большой файл, не больше 5 МБ");

        string extension = Path.GetExtension(file.FileName);
        if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extension))
            return BadRequest("Расширение должно быть jpg, jpeg или png");

        if (index < 1 || index > 4)
            return BadRequest("Индекс должен быть в диапазоне от 1 до 4 включительно");

        try
        {
            bool isExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(eventId.ToString()));

            if (!isExists)
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(eventId.ToString()));

            var listArgs = new ListObjectsArgs()
                .WithBucket(eventId.ToString());

            int count = 0;
            await foreach (var item in _minioClient.ListObjectsEnumAsync(listArgs))
                count++;

            if (count >= 4)
                return BadRequest("Не может быть добавлено больше 4 картинок");


            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                    .WithBucket(eventId.ToString())
                    .WithObject(index.ToString());
                await _minioClient.StatObjectAsync(statObjectArgs);

                return BadRequest("Уже есть картинка с этим индексом");
            }
            catch
            {
                using Stream stream = file.OpenReadStream();
                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(eventId.ToString())
                    .WithObject(index.ToString())
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType));

                return Ok("Картинка успешно добавлена");
            }
        }
        catch (Exception)
        {
            return BadRequest("Minio не запущен");
        }
    }

    /// <summary>
    /// Получение картинки по индексу от 1 до 4 включительно
    /// </summary>
    /// <param name="eventId">Айди события</param>
    /// <param name="index">Индекс</param>
    /// <returns>Файл или сообщение об ошибке</returns>
    [HttpGet("get-image/{eventId:guid}/{index}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageOfEvent(Guid eventId, string index)
    {
        try
        {
            ObjectStat stat = await _minioClient.StatObjectAsync(
            new StatObjectArgs()
                .WithBucket(eventId.ToString())
                .WithObject(index));

            MemoryStream memoryStream = new();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(eventId.ToString())
                .WithObject(index)
                .WithCallbackStream(async stream => await stream.CopyToAsync(memoryStream)));

            memoryStream.Position = 0;

            Console.WriteLine(stat.ContentType);

            return File(memoryStream, stat.ContentType, stat.ContentType.Split("/")[1]);
        }
        catch
        {
            return NotFound("У вас нет картинок или Minio не запущен");
        }
    }

    /// <summary>
    /// Удаляет картинку по индексу от 1 до 4 включительно
    /// </summary>
    /// <param name="eventId">Айди события</param>
    /// <param name="index">Индекс</param>
    /// <returns>Код статуса ответа</returns>
    [HttpDelete("delete-last-image/{eventId:guid}/{index:int}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImageByIndex(Guid eventId, int index)
    {
        if (index < 1 || index > 4)
            return BadRequest("Индекс должен быть в диапазоне от 1 до 4 включительно");

        try
        {
            ObjectStat stat = await _minioClient.StatObjectAsync(
            new StatObjectArgs()
                .WithBucket(eventId.ToString())
                .WithObject(index.ToString()));

            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(eventId.ToString())
                .WithObject(index.ToString()));

            return Ok("Картинка успешно удалена");
        }
        catch
        {
            return NotFound("У события нет картинок, Minio не запущен или нет картинки под этим индексом");
        }
    }
}