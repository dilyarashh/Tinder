using Tinder.DBContext.DTO.User;

namespace Tinder.Services.IServices;

public interface IUserReactionService
{
    Task<ReactionListDTO> GetLikedUsers(string? token);
    Task<ReactionListDTO> GetDislikedUsers(string? token);
    Task<ReactionListDTO> GetMatches(string? token);
    Task<ReactionStatsDTO> GetReactionStats(string? token);
}