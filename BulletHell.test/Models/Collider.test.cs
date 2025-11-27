using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Models;

public class ColliderTests
{
    [Fact]
    public void Constructor_SetsPosition()
    {
        // Arrange
        var pos = new Vector2(10f, 20f);

        // Act
        var collider = new Collider(pos, typeof(object));

        // Assert
        Assert.Equal(pos, collider.Position);
    }

    [Fact]
    public void Default_Radius_IsZero()
    {
        // Arrange & Act
        var collider = new Collider(Vector2.Zero, typeof(object));

        // Assert
        Assert.Equal(0f, collider.Radius);
    }

    [Fact]
    public void Constructor_Sets_Radius()
    {
        // Arrange & Act
        var collider = new Collider(Vector2.Zero, typeof(object), 5.5f);

        // Assert
        Assert.Equal(5.5f, collider.Radius);
    }

    [Fact]
    public void ColliderType_IsSetByConstructor()
    {
        // Arrange & Act
        var collider = new Collider(Vector2.Zero, typeof(object));

        // Assert
        Assert.Equal(typeof(object), collider.ColliderType);
    }

    [Fact]
    public void Position_IsMutable()
    {
        // Arrange
        var collider = new Collider(new Vector2(1, 2), typeof(object));

        // Act
        collider.Position = new Vector2(3, 4);

        // Assert
        Assert.Equal(new Vector2(3, 4), collider.Position);
    }
}
