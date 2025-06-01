using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Common.Models;
using ChatApp.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Users.Queries.LoginUser;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, UserDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserQueryHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto?> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // Create Email value object for validation
        var email = new Email(request.Email);
        
        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);

        if (user == null)
        {
            return null; // User not found
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return null; // Invalid password
        }

        // Update online status
        user.SetOnlineStatus(true);
        await _context.SaveChangesAsync(cancellationToken);

        // Return user DTO
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email.Value,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            IsOnline = user.IsOnline,
            LastSeenAt = user.LastSeenAt,
            CreatedAt = user.CreatedAt,
            Age = user.GetAge()
        };
    }
} 