using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Message>
{
    private readonly IApplicationDbContext _context;

    public SendMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // Verify sender exists
        var sender = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.SenderId, cancellationToken);

        if (sender == null)
        {
            throw new InvalidOperationException($"User with ID {request.SenderId} not found.");
        }

        // Create the message
        var message = new Message(
            request.Content,
            request.SenderId
        );

        // Add to context
        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return message;
    }
} 