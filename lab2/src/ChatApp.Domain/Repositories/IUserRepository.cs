using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<User>> GetOnlineUsersAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
} 