using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tinder.DBContext.DTO;
using Tinder.DBContext.Models;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Token;

namespace Tinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(IMessageService messageService, TokenInteractions tokenService) : Controller
{
    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpPost("send")]
    [SwaggerOperation(Summary = "Отправка сообщения пользователю с которым есть мэтч")]
    [SwaggerResponse(StatusCodes.Status200OK, "Сообщение отправлено")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа (мэтч отсутствует)", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var token = tokenService.GetTokenFromHeader();
        await messageService.SendMessageAsync(token, request);
        return Ok();
    }

    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpGet("chat/{withUserId}")]
    [SwaggerOperation(Summary = "Получение чата с конкретным пользователем (если есть мэтч)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Сообщения получены", typeof(List<MessageDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> GetChat(Guid withUserId)
    {
        var token = tokenService.GetTokenFromHeader();
        var messages = await messageService.GetChatMessagesAsync(token, withUserId);
        return Ok(messages);
    }

    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpPut("edit/{messageId}")]
    [SwaggerOperation(Summary = "Редактирование отправленного сообщения")]
    [SwaggerResponse(StatusCodes.Status200OK, "Сообщение отредактировано")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нельзя редактировать чужое сообщение", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Сообщение не найдено", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditMessageRequest request)
    {
        var token = tokenService.GetTokenFromHeader();
        await messageService.EditMessageAsync(token, messageId, request.NewContent);
        return Ok();
    }

    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpDelete("delete/{messageId}")]
    [SwaggerOperation(Summary = "Удаление отправленного сообщения")]
    [SwaggerResponse(StatusCodes.Status200OK, "Сообщение удалено")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нельзя удалить чужое сообщение", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Сообщение не найдено", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var token = tokenService.GetTokenFromHeader();
        await messageService.DeleteMessageAsync(token, messageId);
        return Ok();
    }
}
