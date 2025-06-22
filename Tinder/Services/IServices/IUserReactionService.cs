using Tinder.DBContext.DTO.User;

namespace Tinder.Services.IServices;

public interface IUserReactionService
{
    Task<List<UserPreviewDTO>> GetLikedUsers(string? token);
    Task<List<UserPreviewDTO>> GetDislikedUsers(string? token);
}