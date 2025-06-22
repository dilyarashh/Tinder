using Microsoft.EntityFrameworkCore;
using Tinder.DBContext;
using Tinder.DBContext.DTO.User;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Token;

namespace Tinder.Services;

public class UserReactionService(AppDbcontext dbcontext, TokenInteractions tokenService) : IUserReactionService
{
    public async Task<List<UserPreviewDTO>> GetLikedUsers(string? token)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var likedUsers = await dbcontext.UserPreferences
            .Where(p => p.FromUserId == userId && p.IsLiked)
            .Select(p => p.ToUser)
            .ToListAsync();

        return likedUsers.Select(u => new UserPreviewDTO
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhotoUrl = u.PhotoUrl,
            Course = u.Course,
            Age = u.Age,
            About = u.About
        }).ToList();
    }

    public async Task<List<UserPreviewDTO>> GetDislikedUsers(string? token)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var dislikedUsers = await dbcontext.UserPreferences
            .Where(p => p.FromUserId == userId && !p.IsLiked)
            .Select(p => p.ToUser)
            .ToListAsync();

        return dislikedUsers.Select(u => new UserPreviewDTO
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhotoUrl = u.PhotoUrl,
            Course = u.Course,
            Age = u.Age,
            About = u.About,
        }).ToList();
    }
}