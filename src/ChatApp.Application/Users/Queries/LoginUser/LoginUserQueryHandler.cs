using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Features.Users.Queries.LoginUser;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, User>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserQueryHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // Create Email value object for validation
        var email = new Email(request.Email);

        // Find user by email
        var user = await _context.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Update online status
        user.SetOnlineStatus(true);
        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }
} 