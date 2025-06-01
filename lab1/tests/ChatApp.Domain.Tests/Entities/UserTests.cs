using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;
using Xunit;

namespace ChatApp.Domain.Tests.Entities;

public class UserTests
{
    private readonly Email _validEmail = new("test@example.com");
    private readonly DateTime _validDateOfBirth = DateTime.Today.AddYears(-25);

    [Fact]
    public void Constructor_WithValidData_ShouldCreateUserSuccessfully()
    {
        // Act
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth);

        // Assert
        Assert.Equal("John Doe", user.Name);
        Assert.Equal(_validEmail, user.Email);
        Assert.Equal("hashedPassword", user.PasswordHash);
        Assert.Equal(Gender.Male, user.Gender);
        Assert.Equal(_validDateOfBirth, user.DateOfBirth);
        Assert.False(user.IsOnline);
        Assert.Null(user.LastSeenAt);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A")] // Too short
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User(invalidName!, _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth));
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User(longName, _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth));
    }

    [Fact]
    public void Constructor_WithNullEmail_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new User("John Doe", null!, "hashedPassword", Gender.Male, _validDateOfBirth));
    }

    [Fact]
    public void Constructor_WithFutureDateOfBirth_ShouldThrowArgumentException()
    {
        // Arrange
        var futureDate = DateTime.Today.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User("John Doe", _validEmail, "hashedPassword", Gender.Male, futureDate));
    }

    [Fact]
    public void Constructor_WithUserTooYoung_ShouldThrowArgumentException()
    {
        // Arrange
        var tooYoungDate = DateTime.Today.AddYears(-12);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User("John Doe", _validEmail, "hashedPassword", Gender.Male, tooYoungDate));
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth);
        var newDateOfBirth = DateTime.Today.AddYears(-30);

        // Act
        user.UpdateProfile("Jane Smith", Gender.Female, newDateOfBirth);

        // Assert
        Assert.Equal("Jane Smith", user.Name);
        Assert.Equal(Gender.Female, user.Gender);
        Assert.Equal(newDateOfBirth, user.DateOfBirth);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void UpdatePassword_WithValidPassword_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = new User("John Doe", _validEmail, "oldHashedPassword", Gender.Male, _validDateOfBirth);

        // Act
        user.UpdatePassword("newHashedPassword");

        // Assert
        Assert.Equal("newHashedPassword", user.PasswordHash);
        Assert.NotNull(user.UpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdatePassword_WithInvalidPassword_ShouldThrowArgumentException(string? invalidPassword)
    {
        // Arrange
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.UpdatePassword(invalidPassword!));
    }

    [Fact]
    public void SetOnlineStatus_ToOnline_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth);

        // Act
        user.SetOnlineStatus(true);

        // Assert
        Assert.True(user.IsOnline);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void SetOnlineStatus_ToOffline_ShouldSetLastSeenAt()
    {
        // Arrange
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, _validDateOfBirth);
        user.SetOnlineStatus(true);

        // Act
        user.SetOnlineStatus(false);

        // Assert
        Assert.False(user.IsOnline);
        Assert.NotNull(user.LastSeenAt);
        Assert.True(user.LastSeenAt <= DateTime.UtcNow);
    }

    [Fact]
    public void GetAge_ShouldCalculateCorrectAge()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-25).AddDays(-1);
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, birthDate);

        // Act
        var age = user.GetAge();

        // Assert
        Assert.Equal(25, age);
    }

    [Fact]
    public void GetAge_BeforeBirthday_ShouldReturnCorrectAge()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-25).AddDays(1);
        var user = new User("John Doe", _validEmail, "hashedPassword", Gender.Male, birthDate);

        // Act
        var age = user.GetAge();

        // Assert
        Assert.Equal(24, age);
    }
} 