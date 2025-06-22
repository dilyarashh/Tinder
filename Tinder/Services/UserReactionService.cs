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
    
    public async Task<List<UserPreviewDTO>> GetMatches(string? token)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var likedByMe = await dbcontext.UserPreferences
            .Where(p => p.FromUserId == userId && p.IsLiked)
            .Select(p => p.ToUserId)
            .ToListAsync();

        var likedMeBack = await dbcontext.UserPreferences
            .Where(p => likedByMe.Contains(p.FromUserId)
                        && p.ToUserId == userId
                        && p.IsLiked)
            .Select(p => p.FromUserId)
            .ToListAsync();

        var matchedUserIds = likedMeBack;

        var matchedUsers = await dbcontext.Users
            .Where(u => matchedUserIds.Contains(u.Id))
            .ToListAsync();

        return matchedUsers.Select(user => new UserPreviewDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhotoUrl = user.PhotoUrl,
            Age = user.Age,
            Course = user.Course,
            About = user.About,
            Telegram = user.Telegram
        }).ToList();
    }

}