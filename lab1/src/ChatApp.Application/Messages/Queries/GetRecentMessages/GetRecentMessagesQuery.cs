using ChatApp.Application.Common.Models;
using MediatR;

namespace ChatApp.Application.Messages.Queries.GetRecentMessages;

public record GetRecentMessagesQuery(
    int Count = 50
) : IRequest<IEnumerable<MessageDto>>; 