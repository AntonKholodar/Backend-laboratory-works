using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Data;

public class ChatAppDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Message> Messages => Set<Message>();

    public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
} 