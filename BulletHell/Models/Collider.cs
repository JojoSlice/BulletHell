using System;
using Microsoft.Xna.Framework;

namespace BulletHell.Models;

public class Collider(Vector2 position, Type? colliderType, float radius = 0f)
{
    public float Radius { get; set; } = radius;
    public Vector2 Position { get; set; } = position;
    public readonly Type? ColliderType = colliderType;

    public bool IsCollidingWith(Collider other)
    {
        if (AreSameCollider(other)
            || AreCollidersOfSameTypeAndNotNull(other)
            || IsPlayerBulletExclusion(other)
            || IsEnemyEnemyBulletExclusion(other)
            || IsBulletEnemyBulletExclusion(other))
        {
            return false;
        }

        return RadiiSquared(this, other) >=
               Vector2.DistanceSquared(Position, other.Position);
    }

    public float Distance(Collider other)
    {
        if (Equals(this, other))
        {
            return 0;
        }

        return Vector2.Distance(Position, other.Position);
    }

    private float RadiiSquared(Collider collider1, Collider collider2) =>
        (collider1.Radius + collider2.Radius) * (collider1.Radius + collider2.Radius);

    private bool AreSameCollider(Collider other) => Equals(this, other);

    private bool AreCollidersOfSameTypeAndNotNull(Collider other) => ColliderType == other.ColliderType
                                                                     && ColliderType != null;

    private bool IsPlayerBulletExclusion(Collider other) => ColliderType == typeof(Player)
                                                            && other.ColliderType == typeof(Bullet);

    private bool IsEnemyEnemyBulletExclusion(Collider other) => ColliderType == typeof(Enemy)
                                                                && other.ColliderType == typeof(EnemyBullet);

    private bool IsBulletEnemyBulletExclusion(Collider other) => ColliderType == typeof(Bullet)
                                                                 && other.ColliderType == typeof(EnemyBullet);
}
