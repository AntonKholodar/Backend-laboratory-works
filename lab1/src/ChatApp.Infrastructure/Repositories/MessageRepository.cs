using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatAppDbContext _context;

    public MessageRepository(ChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetMessagesByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.SenderId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Message message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Message message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);
    }
} 