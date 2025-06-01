using ChatApp.Application.Common.Models;
using MediatR;

namespace ChatApp.Application.Messages.Commands.SendMessage;

public record SendMessageCommand(
    string Content,
    Guid SenderId
) : IRequest<MessageDto>; 