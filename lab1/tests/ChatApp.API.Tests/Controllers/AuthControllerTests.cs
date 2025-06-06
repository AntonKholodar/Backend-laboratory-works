using ChatApp.API.Models.Requests;
using ChatApp.Domain.ValueObjects;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ChatApp.API.Tests.Controllers;

public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Name = "John Doe",
            Email = "invalid-email",
            Password = "password123",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_TooYoung_ReturnsBadRequest()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var request = new RegisterRequest
        {
            Name = "Young User",
            Email = $"young{uniqueId}@example.com",
            Password = "password123",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(2015, 1, 1) // Too young (10 years old)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_EmptyName_ReturnsBadRequest()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var request = new RegisterRequest
        {
            Name = "",
            Email = $"test{uniqueId}@example.com",
            Password = "password123",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShortPassword_ReturnsBadRequest()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var request = new RegisterRequest
        {
            Name = "John Doe",
            Email = $"test{uniqueId}@example.com",
            Password = "123", // Too short
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var loginRequest = new LoginRequest
        {
            Email = $"nonexistent{uniqueId}@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 