using ChatApp.Application.Common.DTOs;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;
using MediatR;

namespace ChatApp.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<User>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
} 