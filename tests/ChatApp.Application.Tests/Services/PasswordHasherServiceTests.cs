using ChatApp.Infrastructure.Services;
using Xunit;

namespace ChatApp.Application.Tests.Services;

public class PasswordHasherServiceTests
{
    private readonly PasswordHasherService _passwordHasher = new();

    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEmpty(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
        Assert.StartsWith("$2a$", hashedPassword); // BCrypt hash format
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HashPassword_WithInvalidPassword_ShouldThrowArgumentException(string? invalidPassword)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordHasher.HashPassword(invalidPassword!));
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(null, "validhash")]
    [InlineData("", "validhash")]
    [InlineData("   ", "validhash")]
    [InlineData("password", null)]
    [InlineData("password", "")]
    [InlineData("password", "   ")]
    public void VerifyPassword_WithInvalidInputs_ShouldReturnFalse(string? password, string? hashedPassword)
    {
        // Act
        var result = _passwordHasher.VerifyPassword(password!, hashedPassword!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHash = "not-a-valid-bcrypt-hash";

        // Act
        var result = _passwordHasher.VerifyPassword(password, invalidHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HashPassword_SamePlaintext_ShouldGenerateDifferentHashes()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2); // BCrypt includes salt, so hashes should be different
        Assert.True(_passwordHasher.VerifyPassword(password, hash1));
        Assert.True(_passwordHasher.VerifyPassword(password, hash2));
    }
} 