using ChatApp.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    /// <summary>
    /// Get information about the chat application
    /// </summary>
    /// <returns>Application information</returns>
    [HttpGet("about")]
    public ActionResult<ApiResponse<object>> GetAboutInfo()
    {
        var appInfo = new
        {
            Name = "ChatApp",
            Version = "1.0.0",
            Description = "A modern chat application built with Clean Architecture and .NET 8",
            Developer = "Anton Kholodar",
            Group = "KV-41mp",
            Features = new[]
            {
                "User Registration and Authentication",
                "Real-time Chat Messaging",
                "User Profile Management",
                "Secure JWT Authentication",
                "SQLite Database Storage",
                "RESTful API Design",
                "Swagger API Documentation"
            },
            Technologies = new[]
            {
                ".NET 8",
                "ASP.NET Core Web API",
                "Entity Framework Core",
                "SQLite",
                "MediatR (CQRS)",
                "FluentValidation",
                "JWT Authentication",
                "BCrypt Password Hashing",
                "Swagger/OpenAPI"
            },
            Architecture = new
            {
                Pattern = "Clean Architecture",
                Layers = new[]
                {
                    "Domain Layer - Entities, Value Objects, Domain Services",
                    "Application Layer - Use Cases, DTOs, Interfaces",
                    "Infrastructure Layer - Data Access, External Services",
                    "API Layer - Controllers, Authentication, Documentation"
                },
                Principles = new[]
                {
                    "Domain-Driven Design (DDD)",
                    "Command Query Responsibility Segregation (CQRS)",
                    "Dependency Inversion",
                    "Single Responsibility Principle",
                    "Separation of Concerns"
                }
            },
            Contact = new
            {
                University = "National Technical University of Ukraine \"Igor Sikorsky Kyiv Polytechnic Institute\"",
                Course = "Backend Development Laboratory Work",
                Assignment = "Laboratory Work #1"
            }
        };

        return Ok(ApiResponse<object>.SuccessResult(appInfo, "Application information retrieved successfully"));
    }

    /// <summary>
    /// Get API health status
    /// </summary>
    /// <returns>Health check information</returns>
    [HttpGet("health")]
    public ActionResult<ApiResponse<object>> GetHealthStatus()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            Version = "1.0.0",
            Uptime = DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime.ToUniversalTime()),
            Database = "SQLite - Connected"
        };

        return Ok(ApiResponse<object>.SuccessResult(healthInfo, "API is healthy"));
    }
} 