using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace BulletHell.Managers;

public class CollisionManager(Collider colliderA, Collider colliderB)
{
    public bool IsColliding()
    {
        if (colliderA.Equals(colliderB)
            || (colliderA.ColliderType == colliderB.ColliderType
                && colliderA.ColliderType != null)
            || (colliderA.ColliderType == typeof(Player) && colliderB.ColliderType == typeof(Bullet))
            || (colliderA.ColliderType == typeof(Enemy) && colliderB.ColliderType == typeof(EnemyBullet))
            || (colliderA.ColliderType == typeof(Bullet) && colliderB.ColliderType == typeof(EnemyBullet)))
        {
            return false;
        }

        return RadiiSquared(colliderA, colliderB) >=
               Vector2.DistanceSquared(colliderA.Position, colliderB.Position);
    }

    public float Distance()
    {
        if (colliderA.Equals(colliderB))
        {
            return 0;
        }

        return Vector2.Distance(colliderA.Position, colliderB.Position);
    }

    private float RadiiSquared(Collider collider1, Collider collider2) =>
        (collider1.Radius + collider2.Radius) * (collider1.Radius + collider2.Radius);
}
