using ChatApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities;

public class Message : BaseEntity
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; private set; } = string.Empty;

    [Required]
    public Guid SenderId { get; private set; }

    public bool IsEdited { get; private set; }

    public DateTime? EditedAt { get; private set; }

    // Navigation properties
    public virtual User Sender { get; private set; } = null!;

    // Private constructor for EF Core
    private Message() { }

    public Message(string content, Guid senderId)
    {
        ValidateContent(content);
        
        if (senderId == Guid.Empty)
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));

        Content = content.Trim();
        SenderId = senderId;
        IsEdited = false;
        EditedAt = null;
    }

    public void UpdateContent(string newContent)
    {
        ValidateContent(newContent);

        if (Content == newContent.Trim())
            return; // No change needed

        Content = newContent.Trim();
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public bool CanBeEditedBy(Guid userId)
    {
        return SenderId == userId;
    }

    public bool IsRecentlyCreated(TimeSpan timeWindow)
    {
        return DateTime.UtcNow - CreatedAt <= timeWindow;
    }

    private static void ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be null or empty.", nameof(content));

        if (content.Trim().Length > 1000)
            throw new ArgumentException("Message content cannot exceed 1000 characters.", nameof(content));
    }
} 