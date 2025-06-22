using Microsoft.EntityFrameworkCore;
using Tinder.DBContext;
using Tinder.DBContext.DTO;
using Tinder.DBContext.DTO.User;
using Tinder.DBContext.Models;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Exceptions;
using Tinder.SupportiveServices.Token;

namespace Tinder.Services;

public class UserPreferenceService (AppDbcontext dbcontext, TokenInteractions tokenService)
    : IUserPreferenceService
{

    public async Task<UserPreviewDTO?> GetNextUser(string? token, FilterDTO filters)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var seenUserIds = await dbcontext.UserPreferences
            .Where(p => p.FromUserId == userId)
            .Select(p => p.ToUserId)
            .ToListAsync();

        var query = dbcontext.Users.AsQueryable();

        query = query.Where(u => u.Id != userId);

        if (filters.EducationLevel.HasValue)
            query = query.Where(u => u.EducationLevel == filters.EducationLevel.Value);

        if (filters.Course.HasValue)
            query = query.Where(u => u.Course == filters.Course.Value);

        if (filters.Gender.HasValue)
            query = query.Where(u => u.Gender == filters.Gender.Value);

        if (filters.MinAge.HasValue)
            query = query.Where(u => u.Age >= filters.MinAge.Value);

        if (filters.MaxAge.HasValue)
            query = query.Where(u => u.Age <= filters.MaxAge.Value);

        query = query.Where(u => !seenUserIds.Contains(u.Id));

        var users = await query.ToListAsync();

        if (!users.Any())
            throw new BadRequestException("Подходящих пользователей не найдено 😔 Попробуй изменить фильтры.");

        var random = new Random();
        var selected = users.OrderBy(_ => random.Next()).First();

        return new UserPreviewDTO
        {
            Id = selected.Id,
            FirstName = selected.FirstName,
            LastName = selected.LastName,
            PhotoUrl = selected.PhotoUrl,
            Course = selected.Course,
            Age = selected.Age,
            About = selected.About,
        };
    }

    public async Task SetPreference(string? token, Guid toUserId, bool isLiked)
    {
        var fromUserId = Guid.Parse(tokenService.GetIdFromToken(token));

        if (fromUserId == toUserId)
            throw new BadRequestException("Нельзя оценить самого себя, даже если ты себя любишь!");

        var exists = await dbcontext.UserPreferences
            .AnyAsync(p => p.FromUserId == fromUserId && p.ToUserId == toUserId);

        if (exists)
            throw new BadRequestException("Вы уже оценили этого пользователя");

        var pref = new UserPreference
        {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            IsLiked = isLiked
        };

        await dbcontext.UserPreferences.AddAsync(pref);
        await dbcontext.SaveChangesAsync();
    }
}