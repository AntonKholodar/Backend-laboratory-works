using ChatApp.Application.Common.Interfaces;
using ChatApp.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatApp.API.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        // Add test configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Testing.json", optional: true);
        });
        
        builder.ConfigureServices(services =>
        {
            // Remove ALL Entity Framework related services
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.Name.Contains("DbContext") ||
                           d.ServiceType.Name.Contains("EntityFramework") ||
                           d.ServiceType == typeof(ChatAppDbContext) ||
                           d.ServiceType == typeof(IApplicationDbContext) ||
                           d.ServiceType == typeof(DbContextOptions<ChatAppDbContext>) ||
                           d.ServiceType == typeof(DbContextOptions) ||
                           d.ImplementationType?.Name.Contains("ChatAppDbContext") == true ||
                           d.ImplementationType?.Name.Contains("SqliteConnection") == true ||
                           d.ServiceType.Name.Contains("Sqlite"))
                .ToArray();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Register test database with InMemory provider - use unique database name per factory instance
            services.AddDbContext<ChatAppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName)
                       .EnableSensitiveDataLogging()
                       .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });

            // Re-register IApplicationDbContext
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ChatAppDbContext>());
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        
        // Ensure database is created after host is built
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatAppDbContext>();
        context.Database.EnsureCreated();
        
        return host;
    }
} 