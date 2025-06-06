using ChatApp.API.Models.Requests;
using ChatApp.API.Models.Responses;
using ChatApp.API.Services;
using ChatApp.Application.Common.DTOs;
using ChatApp.Application.Features.Users.Commands.RegisterUser;
using ChatApp.Application.Features.Users.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IMediator mediator, IJwtTokenService jwtTokenService)
    {
        _mediator = mediator;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>User information and JWT token</returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
    {
        var command = new RegisterUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth
        };

        var user = await _mediator.Send(command);
        var token = _jwtTokenService.GenerateToken(user);

        var response = new AuthResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email.Value,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                IsOnline = user.IsOnline,
                LastSeenAt = user.LastSeenAt,
                CreatedAt = user.CreatedAt
            },
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "User registered successfully"));
    }

    /// <summary>
    /// Authenticate a user
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>User information and JWT token</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
    {
        var query = new LoginUserQuery
        {
            Email = request.Email,
            Password = request.Password
        };

        var user = await _mediator.Send(query);
        var token = _jwtTokenService.GenerateToken(user);

        var response = new AuthResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email.Value,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                IsOnline = user.IsOnline,
                LastSeenAt = user.LastSeenAt,
                CreatedAt = user.CreatedAt
            },
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "Login successful"));
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<UserDto>.ErrorResult("User not authenticated"));
        }

        var userDto = new UserDto
        {
            Id = Guid.Parse(userId),
            Name = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
            Gender = User.FindFirst("gender")?.Value ?? "",
            DateOfBirth = DateTime.Parse(User.FindFirst("dateOfBirth")?.Value ?? DateTime.MinValue.ToString()),
            IsOnline = true,
            LastSeenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "User information retrieved successfully"));
    }
} 