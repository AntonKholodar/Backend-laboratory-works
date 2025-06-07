using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Message> Messages { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 