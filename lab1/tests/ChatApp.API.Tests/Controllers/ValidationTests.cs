using ChatApp.API.Models.Requests;
using ChatApp.Domain.ValueObjects;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ChatApp.API.Tests.Controllers;

public class ValidationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ValidationTests(TestWebApplicationFactory factory)
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
            Email = "invalid-email", // Invalid email format
            Password = "password123",
            Gender = Gender.Male,
            DateOfBirth = DateTime.Today.AddYears(-25)
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
        var request = new RegisterRequest
        {
            Name = "", // Empty name
            Email = "test@example.com",
            Password = "password123",
            Gender = Gender.Male,
            DateOfBirth = DateTime.Today.AddYears(-25)
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
        var request = new RegisterRequest
        {
            Name = "John Doe",
            Email = "test@example.com",
            Password = "123", // Too short password
            Gender = Gender.Male,
            DateOfBirth = DateTime.Today.AddYears(-25)
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
        var request = new RegisterRequest
        {
            Name = "Young User",
            Email = "young@example.com",
            Password = "password123",
            Gender = Gender.Female,
            DateOfBirth = DateTime.Today.AddYears(-10) // Only 10 years old
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_MultipleValidationErrors_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Name = "", // Empty name
            Email = "invalid-email", // Invalid email
            Password = "123", // Too short password
            Gender = Gender.Male,
            DateOfBirth = DateTime.Today.AddYears(-5) // Too young
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
} 