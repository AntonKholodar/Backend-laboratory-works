using ChatApp.API.Models.Responses;
using ChatApp.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace ChatApp.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList()
            },
            DomainException domainEx => new ApiResponse
            {
                Success = false,
                Message = domainEx.Message,
                Errors = new List<string>()
            },
            UnauthorizedAccessException => new ApiResponse
            {
                Success = false,
                Message = "Unauthorized access",
                Errors = new List<string>()
            },
            ArgumentException argEx => new ApiResponse
            {
                Success = false,
                Message = argEx.Message,
                Errors = new List<string>()
            },
            _ => new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request",
                Errors = new List<string>()
            }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            DomainException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
} 