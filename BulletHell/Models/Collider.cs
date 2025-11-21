using System.Numerics;

namespace BulletHell.Models;

public class Collider(Vector2 pos1, Vector2 pos2)
{
    public Vector2 Position1 { get; init; } = pos1;
    public Vector2 Position2 { get; init; } = pos2;

    public float CheckDistance()
    {
        throw new System.NotImplementedException();
    }
}
