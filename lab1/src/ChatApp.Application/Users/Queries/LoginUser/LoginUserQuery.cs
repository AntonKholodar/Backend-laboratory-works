using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.Features.Users.Queries.LoginUser;

public class LoginUserQuery : IRequest<User>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
} 