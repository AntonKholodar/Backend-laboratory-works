using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Common.Models;
using ChatApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Messages.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IApplicationDbContext _context;

    public SendMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // Verify sender exists
        var sender = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.SenderId, cancellationToken);

        if (sender == null)
        {
            throw new InvalidOperationException($"User with ID {request.SenderId} not found.");
        }

        // Create message using domain factory
        var message = new Message(request.Content, request.SenderId);

        // Add to context
        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        // Return DTO with sender information
        return new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            SenderId = message.SenderId,
            SenderName = sender.Name,
            IsEdited = message.IsEdited,
            EditedAt = message.EditedAt,
            CreatedAt = message.CreatedAt
        };
    }
} 