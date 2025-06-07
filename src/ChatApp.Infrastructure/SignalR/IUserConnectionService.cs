namespace ChatApp.Infrastructure.SignalR;

public interface IUserConnectionService
{
    Task UserConnectedAsync(string userId, string connectionId);
    Task UserDisconnectedAsync(string userId, string connectionId);
    Task<IEnumerable<string>> GetUserConnectionsAsync(string userId);
    Task<IEnumerable<string>> GetOnlineUsersAsync();
    Task<int> GetOnlineUserCountAsync();
    Task<bool> IsUserOnlineAsync(string userId);
} 