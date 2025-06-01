using ChatApp.Application.Common.Models;
using MediatR;

namespace ChatApp.Application.Users.Queries.LoginUser;

public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<UserDto?>; 