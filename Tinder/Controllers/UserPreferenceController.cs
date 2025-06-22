using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tinder.DBContext.DTO;
using Tinder.DBContext.DTO.User;
using Tinder.DBContext.Models;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Token;

namespace Tinder.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserPreferenceController (IUserPreferenceService preferenceService, TokenInteractions tokenService) : Controller
{
    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpPost("next")]
    [SwaggerOperation(Summary = "Получение рандомного пользователя")]
    [SwaggerResponse(StatusCodes.Status200OK, "Пользователь получен")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<ActionResult<UserPreviewDTO?>> GetNextUser([FromBody] FilterDTO filters)
    {
        var token = tokenService.GetTokenFromHeader();
        var user = await preferenceService.GetNextUser(token, filters);
        return Ok(user);
    }

    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpPost("set-preference")]
    [SwaggerOperation(Summary = "Проставление лайка/дизлайка пользователю")]
    [SwaggerResponse(StatusCodes.Status200OK, "Лайк/дизлайк поставлен")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибки валидации", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> SetPreference(Guid toUserId, bool isLiked)
    {
        var token = tokenService.GetTokenFromHeader();
        await preferenceService.SetPreference(token, toUserId, isLiked);
        return Ok();
    }
    
    [HttpDelete("{toUserId}")]
    [SwaggerOperation(Summary = "Удаление поставленной реакции")]
    [SwaggerResponse(StatusCodes.Status200OK, "Реакция удалена")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> RemoveReaction(Guid toUserId)
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        await preferenceService.RemoveReaction(token, toUserId);
        return Ok();
    }

}