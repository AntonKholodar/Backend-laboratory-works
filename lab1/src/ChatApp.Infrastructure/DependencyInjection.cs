using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Repositories;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Add Entity Framework Core with SQLite
        services.AddDbContext<ChatAppDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ChatAppDbContext).Assembly.FullName)));

        // Register the DbContext as IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ChatAppDbContext>());

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // Register services
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        return services;
    }
} 