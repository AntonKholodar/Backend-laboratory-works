using ChatApp.Application.Common.DTOs;
using MediatR;

namespace ChatApp.Application.Features.Messages.Queries.GetRecentMessages;

public class GetRecentMessagesQuery : IRequest<List<MessageDto>>
{
    public int Limit { get; set; } = 50;
} 