using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace BulletHell.Managers;

public class CollisionManager(Collider colliderA, Collider colliderB)
{
    public bool IsColliding()
    {
        if (AreSameCollider()
            || AreCollidersOfSameTypeAndNotNull()
            || IsPlayerBulletExclusion()
            || IsEnemyEnemyBulletExclusion()
            || IsBulletEnemyBulletExclusion())
        {
            return false;
        }

        return RadiiSquared(colliderA, colliderB) >=
               Vector2.DistanceSquared(colliderA.Position, colliderB.Position);
    }

    public float Distance()
    {
        if (Equals(colliderA, colliderB))
        {
            return 0;
        }

        return Vector2.Distance(colliderA.Position, colliderB.Position);
    }

    private float RadiiSquared(Collider collider1, Collider collider2) =>
        (collider1.Radius + collider2.Radius) * (collider1.Radius + collider2.Radius);

    private bool AreSameCollider() => Equals(colliderA, colliderB);

    private bool AreCollidersOfSameTypeAndNotNull() => colliderA.ColliderType == colliderB.ColliderType
                                                       && colliderA.ColliderType != null;

    private bool IsPlayerBulletExclusion() => colliderA.ColliderType == typeof(Player)
                                              && colliderB.ColliderType == typeof(Bullet);

    private bool IsEnemyEnemyBulletExclusion() => colliderA.ColliderType == typeof(Enemy)
                                                  && colliderB.ColliderType == typeof(EnemyBullet);

    private bool IsBulletEnemyBulletExclusion() => colliderA.ColliderType == typeof(Bullet)
                                                   && colliderB.ColliderType == typeof(EnemyBullet);
}
