using ChatApp.Application.Common.DTOs;
using ChatApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Features.Messages.Queries.GetRecentMessages;

public class GetRecentMessagesQueryHandler : IRequestHandler<GetRecentMessagesQuery, List<MessageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecentMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> Handle(GetRecentMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Take(request.Limit)
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

        return messages;
    }
} 