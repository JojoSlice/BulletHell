using BulletHell.Interfaces;
using BulletHell.Services;
using Xunit;

namespace BulletHell.test.Services;

public class BCryptPasswordHasherTests
{
    private readonly IPasswordHasher _hasher;

    public BCryptPasswordHasherTests()
    {
        _hasher = new BCryptPasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnNonEmptyString()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "SamePassword";

        // Act
        var hash1 = _hasher.HashPassword(password);
        var hash2 = _hasher.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ShouldNotReturnOriginalPassword()
    {
        // Arrange
        var password = "SecretPassword";

        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var password = "CorrectPassword";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var correctPassword = "CorrectPassword";
        var wrongPassword = "WrongPassword";
        var hash = _hasher.HashPassword(correctPassword);

        // Act
        var result = _hasher.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("VeryLongPasswordWithLotsOfCharacters123456789!@#$%")]
    public void HashPassword_ShouldHandleDifferentPasswordLengths(string password)
    {
        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        var canVerify = _hasher.VerifyPassword(password, hash);
        Assert.True(canVerify);
    }

    [Fact]
    public void HashPassword_ShouldThrowArgumentNullException_WhenPasswordIsNull()
    {
        // Arrange
        string? password = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _hasher.HashPassword(password!));
    }

    [Fact]
    public void VerifyPassword_ShouldThrowArgumentNullException_WhenPasswordIsNull()
    {
        // Arrange
        string? password = null;
        var validHash = _hasher.HashPassword("test");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword(password!, validHash));
    }
}
