using Microsoft.EntityFrameworkCore;
using Tinder.DBContext;
using Tinder.DBContext.DTO;
using Tinder.DBContext.Models;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Exceptions;
using Tinder.SupportiveServices.Token;

namespace Tinder.Services;

public class MessageService(AppDbcontext db, TokenInteractions tokenService) : IMessageService
{
    public async Task SendMessageAsync(string? token, SendMessageRequest request)
    {
        var fromUserId = Guid.Parse(tokenService.GetIdFromToken(token));

        var matchExists = await db.UserPreferences.AnyAsync(p =>
                              p.FromUserId == fromUserId && p.ToUserId == request.ToUserId && p.IsLiked) &&
                          await db.UserPreferences.AnyAsync(p =>
                              p.FromUserId == request.ToUserId && p.ToUserId == fromUserId && p.IsLiked);

        if (!matchExists)
            throw new BadRequestException("Нельзя писать пользователю без взаимного лайка!");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            FromUserId = fromUserId,
            ToUserId = request.ToUserId,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync();
    }

    public async Task<List<MessageDTO>> GetChatMessagesAsync(string? token, Guid withUserId)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var messages = await db.Messages
            .Where(m =>
                (m.FromUserId == userId && m.ToUserId == withUserId) ||
                (m.FromUserId == withUserId && m.ToUserId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return messages.Select(m => new MessageDTO
        {
            FromUserId = m.FromUserId,
            ToUserId = m.ToUserId,
            Content = m.Content,
            SentAt = m.SentAt
        }).ToList();
    }

    public async Task EditMessageAsync(string? token, Guid messageId, string newContent)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var message = await db.Messages.FindAsync(messageId)
                      ?? throw new NotFoundException("Сообщение не найдено");

        if (message.FromUserId != userId)
            throw new BadRequestException("Ты не можешь редактировать чужое сообщение!");

        message.Content = newContent;
        message.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(string? token, Guid messageId)
    {
        var userId = Guid.Parse(tokenService.GetIdFromToken(token));

        var message = await db.Messages.FindAsync(messageId)
                      ?? throw new NotFoundException("Сообщение не найдено");

        if (message.FromUserId != userId)
            throw new BadRequestException("Ты не можешь удалить чужое сообщение!");

        db.Messages.Remove(message);
        await db.SaveChangesAsync();
    }
}
