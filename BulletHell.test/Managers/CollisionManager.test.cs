using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.test.Helpers;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Managers;

public class CollisionManagerTests
{

    [Theory]
    [InlineData(2,2)]
    [InlineData(4,7)]
    [InlineData(6,12)]
    [InlineData(8,17)]
    [InlineData(10,22)]
    [InlineData(12,27)]
    public void CollisionManager_CheckObjectsDistance_ReturnsCorrect(int x, int y)
    {
        var collider1 = new Collider(new Vector2(1, 1));
        var collider2 = new Collider(new Vector2(x, y));
        var expectedDistance = ((collider1.Position.X - collider2.Position.X)
                                * (collider1.Position.X - collider2.Position.X))
                                + ((collider1.Position.Y - collider2.Position.Y)
                                * (collider1.Position.Y - collider2.Position.Y));
        var collMan = new CollisionManager(collider1, collider2);
        var actualDistance = collMan.Distance();

        Assert.Equal(expectedDistance, actualDistance);
    }

    [Fact]
    public void CollisionManager_CheckObjectsDistance_ReturnsZero_WhenObjectsAreSame()
    {
        var collider = new Collider(new Vector2(1, 1));
        var collMan = new CollisionManager(collider, collider);
        var actualDistance = collMan.Distance();
        Assert.Equal(0, actualDistance);
    }

    [Theory]
    [InlineData(2, 2, 2, 2)]
    [InlineData(4, 7, 3, 5)]
    [InlineData(6, 12, 4, 4)]
    [InlineData(8, 17, 5, 5)]
    [InlineData(10, 22, 6, 6)]
    public void CollisionManager_CheckCollision_ReturnsTrueIfTouching(int x, int y, int radius1, int radius2)
    {
        var object1 = new Collider(new Vector2(1, 1));
        var object2 = new Collider(new Vector2(x, y));
        object1.Radius = radius1;
        object2.Radius = radius2;

        var collMan = new CollisionManager(object1, object2);
        var expected = (radius1 + radius2) * (radius1 + radius2) >= collMan.Distance();

        var actual = collMan.IsColliding();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollisionManager_CheckCollision_ReturnsFalse_WhenObjectsAreSame()
    {
        var collider = new Collider(new Vector2(1, 1));
        var collMan = new CollisionManager(collider, collider);

        var actual = collMan.IsColliding();

        Assert.False(actual);
    }

    [Theory]
    [ClassData(typeof(CollisionManagerTestData))]
    public void CollisionManager_ReturnsTrue_WhenColliderTypeIsNotFriendly(Collider collider1, Collider collider2, bool shouldCollide)
    {
        var collMan = new CollisionManager(collider1, collider2);
        Assert.Equal(shouldCollide, collMan.IsColliding());
    }

    // --- Additional tests ---

    [Fact]
    public void Distance_IsSymmetric()
    {
        var a = new Collider(new Vector2(0, 0));
        var b = new Collider(new Vector2(3, 4));

        var ab = new CollisionManager(a, b);
        var ba = new CollisionManager(b, a);

        Assert.Equal(ab.Distance(), ba.Distance());
    }

    [Fact]
    public void IsColliding_IsSymmetric()
    {
        var a = new Collider(new Vector2(0, 0)) { Radius = 2 };
        var b = new Collider(new Vector2(3, 0)) { Radius = 2 };

        var ab = new CollisionManager(a, b);
        var ba = new CollisionManager(b, a);

        Assert.Equal(ab.IsColliding(), ba.IsColliding());
    }

    [Fact]
    public void TouchingBoundary_IsConsideredCollision()
    {
        // place colliders on x axis so squared distance == (r1 + r2)^2
        var a = new Collider(new Vector2(0, 0)) { Radius = 2 };
        var b = new Collider(new Vector2(5, 0)) { Radius = 3 };

        var collMan = new CollisionManager(a, b);

        // exact touching -> should be colliding under >= semantics
        Assert.True(collMan.IsColliding());
    }

    [Fact]
    public void FarApart_IsNotColliding()
    {
        var a = new Collider(new Vector2(0, 0)) { Radius = 1 };
        var b = new Collider(new Vector2(100, 100)) { Radius = 1 };

        var collMan = new CollisionManager(a, b);
        Assert.False(collMan.IsColliding());
    }

    [Fact]
    public void ZeroRadius_InsideOther_IsColliding()
    {
        var a = new Collider(new Vector2(0, 0)) { Radius = 0 };
        var b = new Collider(new Vector2(0, 0)) { Radius = 5 };

        var collMan = new CollisionManager(a, b);
        Assert.True(collMan.IsColliding());
    }

    [Fact]
    public void NullTypes_DoNotAffect_GeometryChecks()
    {
        var a = new Collider(new Vector2(0, 0)) { Radius = 1, ColliderType = null };
        var b = new Collider(new Vector2(10, 0)) { Radius = 1, ColliderType = null };

        var collMan = new CollisionManager(a, b);

        // geometry alone -> not colliding
        Assert.False(collMan.IsColliding());
    }
}
