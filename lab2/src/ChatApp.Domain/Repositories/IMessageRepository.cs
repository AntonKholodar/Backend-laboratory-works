using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Message>> GetRecentMessagesAsync(int count = 50, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Message>> GetMessagesByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(Message message, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Message message, CancellationToken cancellationToken = default);
} 