using ChatApp.Domain.ValueObjects;
using Xunit;

namespace ChatApp.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@domain.co.uk")]
    [InlineData("test123@test-domain.com")]
    public void Constructor_WithValidEmail_ShouldCreateEmailSuccessfully(string validEmail)
    {
        // Act
        var email = new Email(validEmail);

        // Assert
        Assert.Equal(validEmail.ToLowerInvariant(), email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@domain")]
    [InlineData("user.domain.com")]
    public void Constructor_WithInvalidEmail_ShouldThrowArgumentException(string? invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(invalidEmail!));
    }

    [Fact]
    public void ImplicitOperator_ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        string emailString = email;

        // Assert
        Assert.Equal("test@example.com", emailString);
    }

    [Fact]
    public void ImplicitOperator_FromString_ShouldCreateEmail()
    {
        // Act
        Email email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email("TEST@EXAMPLE.COM");

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal("test@example.com", result);
    }
} 