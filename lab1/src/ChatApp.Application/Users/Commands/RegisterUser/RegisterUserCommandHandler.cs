using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using ChatApp.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Create Email value object (this will validate the email format)
        var email = new Email(request.Email);
        
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {email.Value} already exists.");
        }

        // Hash the password
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        // Create the user using domain factory method
        var user = new User(
            request.Name,
            email,
            hashedPassword,
            request.Gender,
            request.DateOfBirth
        );

        // Add to context
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Return the user entity
        return user;
    }
} 