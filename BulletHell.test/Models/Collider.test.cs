using BulletHell.Models;
using BulletHell.test.Helpers;
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

    [Theory]
    [InlineData(2,2)]
    [InlineData(4,7)]
    [InlineData(6,12)]
    [InlineData(8,17)]
    [InlineData(10,22)]
    [InlineData(12,27)]
    public void DistanceMethod_ReturnsCorrect(int x, int y)
    {
        var collider1 = new Collider(new Vector2(1, 1), null, 0f);
        var collider2 = new Collider(new Vector2(x, y), null, 0f);
        var expectedDistance = Vector2.Distance(collider1.Position, collider2.Position);
        var actualDistance = collider1.Distance(collider2);

        Assert.Equal(expectedDistance, actualDistance);
    }

    [Fact]
    public void DistanceMethod_ReturnsZero_WhenObjectsAreSame()
    {
        var collider = new Collider(new Vector2(1, 1), null, 0f);
        var actualDistance = collider.Distance(collider);
        Assert.Equal(0, actualDistance);
    }

    [Theory]
    [InlineData(2, 2, 2, 2)]
    [InlineData(4, 7, 3, 5)]
    [InlineData(6, 12, 4, 4)]
    [InlineData(8, 17, 5, 5)]
    [InlineData(10, 22, 6, 6)]
    public void CheckCollision_ReturnsTrueIfTouching(int x, int y, int radius1, int radius2)
    {
        var object1 = new Collider(new Vector2(1, 1), null, radius1);
        var object2 = new Collider(new Vector2(x, y), null, radius2);

        var expected = (float)(radius1 + radius2) * (radius1 + radius2) >=
                       Vector2.DistanceSquared(object1.Position, object2.Position);

        var actual = object1.IsCollidingWith(object2);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CheckCollision_ReturnsFalse_WhenObjectsAreSame()
    {
        var collider = new Collider(new Vector2(1, 1), null, 0f);

        var actual = collider.IsCollidingWith(collider);

        Assert.False(actual);
    }

    [Theory]
    [ClassData(typeof(CollisionManagerTestData))]
    public void CollisionManager_ReturnsTrue_WhenColliderTypeIsNotFriendly(Collider collider1, Collider collider2, bool shouldCollide) =>
        Assert.Equal(shouldCollide, collider1.IsCollidingWith(collider2));

    // --- Additional tests ---

    [Fact]
    public void Distance_IsSymmetric()
    {
        var a = new Collider(new Vector2(0, 0), null, 0f);
        var b = new Collider(new Vector2(3, 4), null, 0f);

        var ab = a.Distance(b);
        var ba = b.Distance(a);

        Assert.Equal(ab, ba);
    }

    [Fact]
    public void IsColliding_IsSymmetric()
    {
        var a = new Collider(new Vector2(0, 0), null, 2f);
        var b = new Collider(new Vector2(3, 0), null, 2f);

        var ab = a.IsCollidingWith(b);
        var ba = b.IsCollidingWith(a);

        Assert.Equal(ab, ba);
    }

    [Fact]
    public void TouchingBoundary_IsConsideredCollision()
    {
        // place colliders on x axis so squared distance == (r1 + r2)^2
        var a = new Collider(new Vector2(0, 0), null, 2f);
        var b = new Collider(new Vector2(5, 0), null, 3f);


        // exact touching -> should be colliding under >= semantics
        Assert.True(a.IsCollidingWith(b));
        Assert.True(b.IsCollidingWith(a));
    }

    [Fact]
    public void FarApart_IsNotColliding()
    {
        var a = new Collider(new Vector2(0, 0), null, 1f);
        var b = new Collider(new Vector2(100, 100), null, 1f);

        Assert.False(a.IsCollidingWith(b));
        Assert.False(b.IsCollidingWith(a));
    }

    [Fact]
    public void ZeroRadius_InsideOther_IsColliding()
    {
        var a = new Collider(new Vector2(0, 0), null, 0f);
        var b = new Collider(new Vector2(0, 0), null, 5f);

        Assert.True(a.IsCollidingWith(b));
        Assert.True(b.IsCollidingWith(a));
    }

    [Fact]
    public void NullTypes_DoNotAffect_GeometryChecks()
    {
        var a = new Collider(new Vector2(0, 0), null, 1f);
        var b = new Collider(new Vector2(10, 0), null, 1f);

        // geometry alone -> not colliding
        Assert.False(a.IsCollidingWith(b));
        Assert.False(b.IsCollidingWith(a));
    }
}
