using Microsoft.Extensions.Caching.Memory;

namespace ChatApp.Infrastructure.SignalR;

public class UserConnectionService : IUserConnectionService
{
    private readonly IMemoryCache _cache;
    private const string USER_CONNECTIONS_KEY = "user_connections";
    private const string ONLINE_USERS_KEY = "online_users";

    public UserConnectionService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task UserConnectedAsync(string userId, string connectionId)
    {
        // Get or create user connections dictionary
        var userConnections = _cache.GetOrCreate(USER_CONNECTIONS_KEY, _ => new Dictionary<string, HashSet<string>>())!;
        
        // Add connection for user
        if (!userConnections.ContainsKey(userId))
            userConnections[userId] = new HashSet<string>();
        
        userConnections[userId].Add(connectionId);

        // Update online users set
        var onlineUsers = _cache.GetOrCreate(ONLINE_USERS_KEY, _ => new HashSet<string>())!;
        onlineUsers.Add(userId);

        return Task.CompletedTask;
    }

    public Task UserDisconnectedAsync(string userId, string connectionId)
    {
        // Get user connections dictionary
        var userConnections = _cache.Get<Dictionary<string, HashSet<string>>>(USER_CONNECTIONS_KEY);
        if (userConnections?.ContainsKey(userId) == true)
        {
            userConnections[userId].Remove(connectionId);

            // If user has no more connections, remove from online users
            if (userConnections[userId].Count == 0)
            {
                userConnections.Remove(userId);
                
                var onlineUsers = _cache.Get<HashSet<string>>(ONLINE_USERS_KEY);
                onlineUsers?.Remove(userId);
            }
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetUserConnectionsAsync(string userId)
    {
        var userConnections = _cache.Get<Dictionary<string, HashSet<string>>>(USER_CONNECTIONS_KEY);
        
        if (userConnections?.ContainsKey(userId) == true)
            return Task.FromResult<IEnumerable<string>>(userConnections[userId]);
        
        return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
    }

    public Task<IEnumerable<string>> GetOnlineUsersAsync()
    {
        var onlineUsers = _cache.Get<HashSet<string>>(ONLINE_USERS_KEY) ?? new HashSet<string>();
        return Task.FromResult<IEnumerable<string>>(onlineUsers);
    }

    public Task<int> GetOnlineUserCountAsync()
    {
        var onlineUsers = _cache.Get<HashSet<string>>(ONLINE_USERS_KEY) ?? new HashSet<string>();
        return Task.FromResult(onlineUsers.Count);
    }

    public Task<bool> IsUserOnlineAsync(string userId)
    {
        var onlineUsers = _cache.Get<HashSet<string>>(ONLINE_USERS_KEY) ?? new HashSet<string>();
        return Task.FromResult(onlineUsers.Contains(userId));
    }
} 