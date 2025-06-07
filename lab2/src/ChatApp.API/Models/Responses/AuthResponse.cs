using ChatApp.Application.Common.DTOs;

namespace ChatApp.API.Models.Responses;

public class AuthResponse
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
} 