using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ChatApp.Infrastructure.SignalR;

namespace ChatApp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IUserConnectionService _connectionService;

    public ChatHub(IUserConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task SendMessage(string message)
    {
        var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        await Clients.All.SendAsync("ReceiveMessage", new
        {
            UserId = userId,
            UserName = userName,
            Message = message,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task JoinAdminGroup()
    {
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userRole == "Admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            await Clients.Caller.SendAsync("JoinedAdminGroup");
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        // Track user connection
        if (!string.IsNullOrEmpty(userId))
        {
            await _connectionService.UserConnectedAsync(userId, Context.ConnectionId);
        }

        // Add user to appropriate groups
        await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
        
        if (userRole == "Admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        // Notify others about user coming online
        await Clients.Others.SendAsync("UserConnected", new
        {
            UserId = userId,
            UserName = userName,
            Timestamp = DateTime.UtcNow
        });

        // Notify admins about user status change
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group("Admins").SendAsync("UserStatusChanged", new
            {
                UserId = userId,
                UserName = userName,
                IsOnline = true,
                Timestamp = DateTime.UtcNow
            });
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        // Track user disconnection
        if (!string.IsNullOrEmpty(userId))
        {
            await _connectionService.UserDisconnectedAsync(userId, Context.ConnectionId);
        }

        // Notify others about user going offline
        await Clients.Others.SendAsync("UserDisconnected", new
        {
            UserId = userId,
            UserName = userName,
            Timestamp = DateTime.UtcNow
        });

        // Notify admins about user status change
        if (!string.IsNullOrEmpty(userId))
        {
            // Check if user is still online (has other connections)
            var isStillOnline = await _connectionService.IsUserOnlineAsync(userId);
            
            if (!isStillOnline)
            {
                await Clients.Group("Admins").SendAsync("UserStatusChanged", new
                {
                    UserId = userId,
                    UserName = userName,
                    IsOnline = false,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
} 