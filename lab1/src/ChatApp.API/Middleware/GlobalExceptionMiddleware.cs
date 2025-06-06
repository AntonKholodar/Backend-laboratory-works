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
                Errors = new List<string> { domainEx.Message }
            },
            UnauthorizedAccessException => new ApiResponse
            {
                Success = false,
                Message = "Access denied",
                Errors = new List<string> { "You do not have permission to access this resource" }
            },
            ArgumentException argEx => new ApiResponse
            {
                Success = false,
                Message = "Invalid argument",
                Errors = new List<string> { argEx.Message }
            },
            InvalidOperationException invalidOpEx => new ApiResponse
            {
                Success = false,
                Message = "Invalid operation",
                Errors = new List<string> { invalidOpEx.Message }
            },
            KeyNotFoundException notFoundEx => new ApiResponse
            {
                Success = false,
                Message = "Resource not found",
                Errors = new List<string> { notFoundEx.Message }
            },
            _ => new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred",
                Errors = new List<string> { "Please try again later or contact support if the problem persists" }
            }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            DomainException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
} 