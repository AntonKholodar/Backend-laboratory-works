using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ChatAppDbContext context)
    {
        // Check if data already exists
        if (await context.Users.AnyAsync())
            return;

        // Create sample users
        var adminUser = new User(
            name: "Admin User",
            email: new Email("admin@chatapp.com"),
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            gender: Gender.Male,
            dateOfBirth: new DateTime(1990, 1, 1),
            role: UserRole.Admin
        );

        var regularUser1 = new User(
            name: "John Doe",
            email: new Email("john.doe@example.com"),
            passwordHash: BCrypt.Net.BCrypt.HashPassword("User123!"),
            gender: Gender.Male,
            dateOfBirth: new DateTime(1995, 5, 15)
        );

        var regularUser2 = new User(
            name: "Jane Smith",
            email: new Email("jane.smith@example.com"),
            passwordHash: BCrypt.Net.BCrypt.HashPassword("User123!"),
            gender: Gender.Female,
            dateOfBirth: new DateTime(1992, 8, 22)
        );

        var regularUser3 = new User(
            name: "Bob Johnson",
            email: new Email("bob.johnson@example.com"),
            passwordHash: BCrypt.Net.BCrypt.HashPassword("User123!"),
            gender: Gender.Male,
            dateOfBirth: new DateTime(1988, 12, 10)
        );

        // Add users to context
        await context.Users.AddRangeAsync(adminUser, regularUser1, regularUser2, regularUser3);

        // Create some sample messages
        var message1 = new Message(
            content: "Welcome to the chat! I'm the admin.",
            senderId: adminUser.Id
        );

        var message2 = new Message(
            content: "Hello everyone! Nice to meet you all.",
            senderId: regularUser1.Id
        );

        var message3 = new Message(
            content: "Hi John! Welcome to our chat app.",
            senderId: regularUser2.Id
        );

        await context.Messages.AddRangeAsync(message1, message2, message3);

        // Save changes
        await context.SaveChangesAsync();
    }
} 