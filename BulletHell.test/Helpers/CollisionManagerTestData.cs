using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Helpers;

public class CollisionManagerTestData : TheoryData<Collider, Collider, bool>
{
    public CollisionManagerTestData()
    {
        Add(new Collider(Vector2.One) { ColliderType = typeof(Player) },
            new Collider(Vector2.One) { ColliderType = typeof(Bullet) },
            false);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Player) },
            new Collider(Vector2.One) { ColliderType = typeof(Enemy) },
            true);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Player) },
            new Collider(Vector2.One) { ColliderType = typeof(EnemyBullet) },
            true);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Bullet) },
            new Collider(Vector2.One) { ColliderType = typeof(Enemy) },
            true);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Bullet) },
            new Collider(Vector2.One) { ColliderType = typeof(EnemyBullet) },
            false);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Enemy) },
            new Collider(Vector2.One) { ColliderType = typeof(EnemyBullet) },
            false);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Player) },
            new Collider(Vector2.One) { ColliderType = typeof(Player) },
            false);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Bullet) },
            new Collider(Vector2.One) { ColliderType = typeof(Bullet) },
            false);

        Add(new Collider(Vector2.One) { ColliderType = typeof(Enemy) },
            new Collider(Vector2.One) { ColliderType = typeof(Enemy) },
            false);

        Add(new Collider(Vector2.One) { ColliderType = typeof(EnemyBullet) },
            new Collider(Vector2.One) { ColliderType = typeof(EnemyBullet) },
            false);
    }
}
