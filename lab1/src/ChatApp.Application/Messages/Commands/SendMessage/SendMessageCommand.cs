using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommand : IRequest<Message>
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
} 