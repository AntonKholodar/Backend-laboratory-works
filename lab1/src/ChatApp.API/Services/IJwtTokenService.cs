using ChatApp.Domain.Entities;

namespace ChatApp.API.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
} 