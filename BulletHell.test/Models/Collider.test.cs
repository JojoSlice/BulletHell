using System.Numerics;
using BulletHell.Models;

namespace BulletHell.test.Models;

public class ColliderTest
{
    [Theory]
    [InlineData(2,2)]
    [InlineData(4,7)]
    [InlineData(6,12)]
    [InlineData(8,17)]
    [InlineData(10,22)]
    [InlineData(12,27)]
    public void Collider_CheckObjectsDistance_ReturnsCorrect(int x, int y)
    {
        var object1 = new Vector2(1, 1);
        var object2 = new Vector2(x, y);
        var expectedDistance = ((object1.X - object2.X) * (object1.X - object1.X)) + ((object1.Y - object2.Y) * (object1.Y - object2.Y));
        var collider = new Collider(object1, object2);

        var actualDistance = collider.Distance();

        Assert.Equal(expectedDistance, actualDistance);
    }

    [Fact]
    public void Collider_CheckObjectsDistance_ReturnsZero_WhenObjectsAreSame()
    {
        var object1 = new Vector2(1, 1);
        var collider = new Collider(object1, object1);
        var actualDistance = collider.Distance();
        Assert.Equal(0, actualDistance);
    }

    [Theory]
    [InlineData(2, 2, 2, 2)]
    [InlineData(4, 7, 3, 5)]
    [InlineData(6, 12, 4, 4)]
    [InlineData(8, 17, 5, 5)]
    [InlineData(10, 22, 6, 6)]
    public void Collider_CheckCollision_ReturnsTrueIfTouching(int x, int y, int radius1, int radius2)
    {
        var object1 = new Vector2(1, 1);
        var object2 = new Vector2(x, y);
        var collider = new Collider(object1, object2);
        var expected = (radius1 + radius2) * (radius1 + radius2) > collider.Distance();

        var actual = collider.IsColliding();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Collider_CheckCollision_ReturnsFalse_WhenObjectsAreSame()
    {
        var object1 = new Vector2(1, 1);
        var collider = new Collider(object1, object1);

        var actual = collider.IsColliding();

        Assert.False(actual);
    }
}
