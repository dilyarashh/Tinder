using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tinder.DBContext.Models;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Token;

namespace Tinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserReactionController(IUserReactionService reactionService, TokenInteractions tokenService) : ControllerBase
{
    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpGet("liked")]
    [SwaggerOperation(Summary = "Получение пользователей, которым поставлен лайк")]
    [SwaggerResponse(StatusCodes.Status200OK, "Данные получены")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> GetLikedUsers()
    {
        var token = tokenService.GetTokenFromHeader();
        var users = await reactionService.GetLikedUsers(token);
        return Ok(users);
    }

    [Authorize(Policy = "TokenBlackListPolicy")]
    [HttpGet("disliked")]
    [SwaggerOperation(Summary = "Получение пользователей, которым поставлен дизлайк")]
    [SwaggerResponse(StatusCodes.Status200OK, "Данные получены")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Пользователь не авторизован", typeof(Error))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ошибка сервера")]
    public async Task<IActionResult> GetDislikedUsers()
    {
        var token = tokenService.GetTokenFromHeader();
        var users = await reactionService.GetDislikedUsers(token);
        return Ok(users);
    }
}