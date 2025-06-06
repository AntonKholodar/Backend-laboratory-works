using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using ChatApp.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Create Email value object (this will validate the email format)
        var email = new Email(request.Email);
        
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
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

        // Add using repository
        try
        {
            return await _userRepository.AddAsync(user, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("UNIQUE constraint") == true)
        {
            throw new InvalidOperationException($"User with email {email.Value} already exists.");
        }
    }
} 