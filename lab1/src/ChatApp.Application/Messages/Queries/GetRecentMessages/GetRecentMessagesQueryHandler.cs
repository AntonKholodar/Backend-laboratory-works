using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Messages.Queries.GetRecentMessages;

public class GetRecentMessagesQueryHandler : IRequestHandler<GetRecentMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecentMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetRecentMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Take(request.Count)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderName = m.Sender.Name,
                IsEdited = m.IsEdited,
                EditedAt = m.EditedAt,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);

        // Return in chronological order (oldest first for chat display)
        return messages.AsEnumerable().Reverse();
    }
} 