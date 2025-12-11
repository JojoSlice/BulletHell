using BulletHell.Factories;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using Xunit;

namespace BulletHell.test.Factories;

public class SpriteHelperFactoryTests
{
    [Fact]
    public void Create_ShouldReturnISpriteHelperInstance()
    {
        // Arrange
        var factory = new SpriteHelperFactory();

        // Act
        var result = factory.Create();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ISpriteHelper>(result);
    }

    [Fact]
    public void Create_ShouldReturnSpriteHelperInstance()
    {
        // Arrange
        var factory = new SpriteHelperFactory();

        // Act
        var result = factory.Create();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SpriteHelper>(result);
    }

    [Fact]
    public void Create_ShouldReturnNewInstanceEachTime()
    {
        // Arrange
        var factory = new SpriteHelperFactory();

        // Act
        var result1 = factory.Create();
        var result2 = factory.Create();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotSame(result1, result2);
    }
}
