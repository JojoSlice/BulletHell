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

        var actualDistance = collider.CheckDistance();

        Assert.Equal(expectedDistance, actualDistance);
    }
}
