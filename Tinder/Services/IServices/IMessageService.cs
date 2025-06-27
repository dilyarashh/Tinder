using Tinder.DBContext.DTO;

namespace Tinder.Services.IServices;

public interface IMessageService
{
    Task SendMessageAsync(string? token, SendMessageRequest request);
    Task<List<MessageDTO>> GetChatMessagesAsync(string? token, Guid withUserId);
    Task EditMessageAsync(string? token, Guid messageId, string newContent);
    Task DeleteMessageAsync(string? token, Guid messageId);
}