using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Configure Email value object
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(256);

        // Create index on the email column
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Gender)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.DateOfBirth)
            .IsRequired();

        builder.Property(u => u.IsOnline)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.LastSeenAt);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        // Configure relationships
        builder.HasMany(u => u.Messages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 