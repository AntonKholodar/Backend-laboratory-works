using ChatApp.Application.Features.Users.Commands.RegisterUser;
using ChatApp.Domain.ValueObjects;
using Xunit;

namespace ChatApp.Application.Tests.Users.Commands;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "", 
            Email = "test@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_ShortName_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "A", 
            Email = "test@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "invalid-email", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
    }

    [Fact]
    public void Validate_ShortPassword_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "test@example.com", 
            Password = "12345", 
            Gender = Gender.Male, 
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_UserTooYoung_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "test@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = DateTime.Today.AddYears(-12)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.DateOfBirth));
    }

    [Fact]
    public void Validate_FutureDateOfBirth_ShouldHaveValidationError()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "test@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = DateTime.Today.AddDays(1)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.DateOfBirth));
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "john.doe@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var result = _validator.Validate(command);
        Assert.True(result.IsValid, $"Validation failed: {string.Join(", ", result.Errors.Select(e => e.ErrorMessage))}");
    }

    [Fact]
    public void Validate_TodayMinus25Years_ShouldPass()
    {
        var birthDate = DateTime.Today.AddYears(-25);
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "john.doe@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = birthDate
        };
        
        var result = _validator.Validate(command);
        
        // Calculate age for debugging
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        
        Assert.True(result.IsValid, 
            $"Validation failed for age {age} (birthDate: {birthDate:yyyy-MM-dd}, today: {today:yyyy-MM-dd}). " +
            $"Errors: {string.Join(", ", result.Errors.Select(e => e.ErrorMessage))}");
    }

    [Fact]
    public void Validate_FixedDate1990_ShouldPass()
    {
        var birthDate = new DateTime(1990, 1, 1);
        var command = new RegisterUserCommand 
        { 
            Name = "John Doe", 
            Email = "john.doe@example.com", 
            Password = "password123", 
            Gender = Gender.Male, 
            DateOfBirth = birthDate
        };
        
        var result = _validator.Validate(command);
        
        // Calculate age for debugging
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        
        Assert.True(result.IsValid, 
            $"Validation failed for age {age} (birthDate: {birthDate:yyyy-MM-dd}, today: {today:yyyy-MM-dd}). " +
            $"Errors: {string.Join(", ", result.Errors.Select(e => e.ErrorMessage))}");
    }
} 