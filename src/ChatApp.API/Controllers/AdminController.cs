using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Infrastructure.SignalR;
using ChatApp.Application.Users.Queries.GetAllUsers;
using MediatR;
using System.Security.Claims;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserConnectionService _connectionService;
    private readonly IMediator _mediator;

    public AdminController(IUserConnectionService connectionService, IMediator mediator)
    {
        _connectionService = connectionService;
        _mediator = mediator;
    }

    [HttpGet("online-users")]
    public async Task<IActionResult> GetOnlineUsers()
    {
        var onlineUserIds = await _connectionService.GetOnlineUsersAsync();
        var onlineUserCount = await _connectionService.GetOnlineUserCountAsync();

        return Ok(new
        {
            OnlineUserIds = onlineUserIds,
            OnlineUserCount = onlineUserCount,
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("online-users/detailed")]
    public async Task<IActionResult> GetOnlineUsersDetailed()
    {
        try
        {
            // Get all users from database
            var allUsersQuery = new GetAllUsersQuery();
            var allUsers = await _mediator.Send(allUsersQuery);

            // Get online user IDs
            var onlineUserIds = (await _connectionService.GetOnlineUsersAsync()).ToHashSet();

            // Filter and map online users
            var onlineUsers = allUsers
                .Where(user => onlineUserIds.Contains(user.Id.ToString()))
                .Select(user => new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsOnline = true,
                    LastSeenAt = user.LastSeenAt
                })
                .ToList();

            return Ok(new
            {
                OnlineUsers = onlineUsers,
                OnlineUserCount = onlineUsers.Count,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error retrieving online users", Error = ex.Message });
        }
    }

    [HttpGet("users/status/{userId}")]
    public async Task<IActionResult> GetUserStatus(string userId)
    {
        var isOnline = await _connectionService.IsUserOnlineAsync(userId);
        var connections = await _connectionService.GetUserConnectionsAsync(userId);

        return Ok(new
        {
            UserId = userId,
            IsOnline = isOnline,
            ConnectionCount = connections.Count(),
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetAdminStats()
    {
        try
        {
            var onlineUserCount = await _connectionService.GetOnlineUserCountAsync();
            
            // Get total user count
            var allUsersQuery = new GetAllUsersQuery();
            var allUsers = await _mediator.Send(allUsersQuery);
            var totalUserCount = allUsers.Count();

            return Ok(new
            {
                TotalUsers = totalUserCount,
                OnlineUsers = onlineUserCount,
                OfflineUsers = totalUserCount - onlineUserCount,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error retrieving admin stats", Error = ex.Message });
        }
    }
} 