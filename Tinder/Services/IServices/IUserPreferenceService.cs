using Tinder.DBContext.DTO;
using Tinder.DBContext.DTO.User;

namespace Tinder.Services.IServices;

public interface IUserPreferenceService
{
    Task<UserPreviewDTO?> GetNextUser(string? token, FilterDTO filters);
    Task SetPreference(string? token, Guid toUserId, bool isLiked);
}