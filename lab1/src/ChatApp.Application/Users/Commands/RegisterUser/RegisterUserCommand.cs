using ChatApp.Application.Common.Models;
using ChatApp.Domain.ValueObjects;
using MediatR;

namespace ChatApp.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    Gender Gender,
    DateTime DateOfBirth
) : IRequest<UserDto>; 