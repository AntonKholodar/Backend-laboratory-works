using ChatApp.Domain.Common;
using ChatApp.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; private set; } = string.Empty;

    [Required]
    public Email Email { get; private set; }

    [Required]
    public string PasswordHash { get; private set; } = string.Empty;

    [Required]
    public Gender Gender { get; private set; }

    [Required]
    public DateTime DateOfBirth { get; private set; }

    public bool IsOnline { get; private set; }

    public DateTime? LastSeenAt { get; private set; }

    // Navigation properties
    public virtual ICollection<Message> Messages { get; private set; } = new List<Message>();

    // Private constructor for EF Core
    private User() 
    { 
        Email = new Email("placeholder@example.com"); // Will be overridden by EF Core
    }

    public User(string name, Email email, string passwordHash, Gender gender, DateTime dateOfBirth)
    {
        ValidateName(name);
        ValidateDateOfBirth(dateOfBirth);

        Name = name.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Gender = gender;
        DateOfBirth = dateOfBirth;
        IsOnline = false;
        LastSeenAt = null;
    }

    public void UpdateProfile(string name, Gender gender, DateTime dateOfBirth)
    {
        ValidateName(name);
        ValidateDateOfBirth(dateOfBirth);

        Name = name.Trim();
        Gender = gender;
        DateOfBirth = dateOfBirth;
        SetUpdatedAt();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        SetUpdatedAt();
    }

    public void SetOnlineStatus(bool isOnline)
    {
        IsOnline = isOnline;
        
        if (!isOnline)
        {
            LastSeenAt = DateTime.UtcNow;
        }
        
        SetUpdatedAt();
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;
            
        return age;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        if (name.Trim().Length < 2)
            throw new ArgumentException("Name must be at least 2 characters long.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.", nameof(name));
    }

    private static void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        if (dateOfBirth == default)
            throw new ArgumentException("Date of birth cannot be default value.", nameof(dateOfBirth));

        if (dateOfBirth > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        var minDate = DateTime.Today.AddYears(-120);
        if (dateOfBirth < minDate)
            throw new ArgumentException("Date of birth cannot be more than 120 years ago.", nameof(dateOfBirth));

        var maxAge = DateTime.Today.AddYears(-13);
        if (dateOfBirth > maxAge)
            throw new ArgumentException("User must be at least 13 years old.", nameof(dateOfBirth));
    }
} 