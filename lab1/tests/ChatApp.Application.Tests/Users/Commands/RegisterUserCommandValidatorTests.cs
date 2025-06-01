using ChatApp.Application.Users.Commands.RegisterUser;
using ChatApp.Domain.ValueObjects;
using Xunit;

namespace ChatApp.Application.Tests.Users.Commands;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand("", "test@example.com", "password123", Gender.Male, DateTime.Today.AddYears(-25));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_ShortName_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand("A", "test@example.com", "password123", Gender.Male, DateTime.Today.AddYears(-25));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand("John Doe", "invalid-email", "password123", Gender.Male, DateTime.Today.AddYears(-25));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
    }

    [Fact]
    public void Validate_ShortPassword_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand("John Doe", "test@example.com", "12345", Gender.Male, DateTime.Today.AddYears(-25));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_UserTooYoung_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand("John Doe", "test@example.com", "password123", Gender.Male, DateTime.Today.AddYears(-12));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.DateOfBirth));
    }

    [Fact]
    public void Validate_FutureDateOfBirth_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand("John Doe", "test@example.com", "password123", Gender.Male, DateTime.Today.AddDays(1));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.DateOfBirth));
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new RegisterUserCommand("John Doe", "test@example.com", "password123", Gender.Male, DateTime.Today.AddYears(-25));
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
} 