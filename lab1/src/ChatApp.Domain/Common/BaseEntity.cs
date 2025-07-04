using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; protected set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; protected set; }

    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
} 