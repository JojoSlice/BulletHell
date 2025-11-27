using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Helpers;

public class CollisionManagerTestData : TheoryData<Collider, Collider, bool>
{
    public CollisionManagerTestData()
    {
        Add(new Collider(Vector2.One, typeof(Player)),
            new Collider(Vector2.One, typeof(Bullet)),
            false);

        Add(new Collider(Vector2.One, typeof(Player)),
            new Collider(Vector2.One, typeof(Enemy)),
            true);

        Add(new Collider(Vector2.One, typeof(Player)),
            new Collider(Vector2.One, typeof(EnemyBullet)),
            true);

        Add(new Collider(Vector2.One, typeof(Bullet)),
            new Collider(Vector2.One, typeof(Enemy)),
            true);

        Add(new Collider(Vector2.One, typeof(Bullet)),
            new Collider(Vector2.One, typeof(EnemyBullet)),
            false);

        Add(new Collider(Vector2.One, typeof(Enemy)),
            new Collider(Vector2.One, typeof(EnemyBullet)),
            false);

        Add(new Collider(Vector2.One, typeof(Player)),
            new Collider(Vector2.One, typeof(Player)),
            false);

        Add(new Collider(Vector2.One, typeof(Bullet)),
            new Collider(Vector2.One, typeof(Bullet)),
            false);

        Add(new Collider(Vector2.One, typeof(Enemy)),
            new Collider(Vector2.One, typeof(Enemy)),
            false);

        Add(new Collider(Vector2.One, typeof(EnemyBullet)),
            new Collider(Vector2.One, typeof(EnemyBullet)),
            false);
    }
}
