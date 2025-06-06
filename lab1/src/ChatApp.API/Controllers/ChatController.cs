using ChatApp.API.Models.Requests;
using ChatApp.API.Models.Responses;
using ChatApp.Application.Common.DTOs;
using ChatApp.Application.Features.Messages.Commands.SendMessage;
using ChatApp.Application.Features.Messages.Queries.GetRecentMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Send a message to the chat
    /// </summary>
    /// <param name="request">Message content</param>
    /// <returns>Sent message details</returns>
    [HttpPost("messages")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage(SendMessageRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<MessageDto>.ErrorResult("User not authenticated"));
            }

            var command = new SendMessageCommand
            {
                SenderId = Guid.Parse(userId),
                Content = request.Content
            };

            var message = await _mediator.Send(command);

            var messageDto = new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                CreatedAt = message.CreatedAt
            };

            return Ok(ApiResponse<MessageDto>.SuccessResult(messageDto, "Message sent successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<MessageDto>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// Get recent messages from the chat
    /// </summary>
    /// <param name="limit">Number of messages to retrieve (default: 50, max: 100)</param>
    /// <returns>List of recent messages</returns>
    [HttpGet("messages")]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetRecentMessages([FromQuery] int limit = 50)
    {
        try
        {
            // Ensure limit is within reasonable bounds
            limit = Math.Min(Math.Max(limit, 1), 100);

            var query = new GetRecentMessagesQuery { Limit = limit };
            var messages = await _mediator.Send(query);

            return Ok(ApiResponse<List<MessageDto>>.SuccessResult(messages, $"Retrieved {messages.Count} messages"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<MessageDto>>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// Get chat statistics
    /// </summary>
    /// <returns>Chat statistics</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetChatStats()
    {
        try
        {
            // Get recent messages to calculate stats
            var query = new GetRecentMessagesQuery { Limit = 1000 }; // Get more for accurate stats
            var messages = await _mediator.Send(query);

            var stats = new
            {
                TotalMessages = messages.Count,
                ActiveUsers = messages.Select(m => m.SenderId).Distinct().Count(),
                LastMessageAt = messages.FirstOrDefault()?.CreatedAt,
                MessagesToday = messages.Count(m => m.CreatedAt.Date == DateTime.Today)
            };

            return Ok(ApiResponse<object>.SuccessResult(stats, "Chat statistics retrieved"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
} 