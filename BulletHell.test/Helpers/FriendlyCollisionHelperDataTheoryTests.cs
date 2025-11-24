using BulletHell.Helpers;
using BulletHell.Models;
using BulletHell.test.Inputs;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Helpers;

public static class FriendlyCollisionHelperDataTheoryTests
{
    private static readonly Player _player = new Player(new Vector2(1, 1), new FakeInputReader(""), new SpriteHelper());
    private static readonly Bullet _bullet1 = new Bullet(new Vector2(1, 1), new Vector2(1, 1), new SpriteHelper());
    private static readonly Bullet _bullet2 = new Bullet(new Vector2(1, 1), new Vector2(1, 1), new SpriteHelper());
    private static readonly Enemy _enemy1 = new Enemy(new Vector2(1, 1), new SpriteHelper());
    private static readonly Enemy _enemy2 = new Enemy(new Vector2(1, 1), new SpriteHelper());
    private static readonly EnemyBullet _enemyBullet1 = new EnemyBullet(new Vector2(1, 1), new Vector2(1, 1), new SpriteHelper());
    private static readonly EnemyBullet _enemyBullet2 = new EnemyBullet(new Vector2(1, 1), new Vector2(1, 1), new SpriteHelper());

    public static readonly TheoryData<Player, Bullet, bool> PlayerBulletCollisionData =
        new() { { _player, _bullet1, false } };

    public static readonly TheoryData<Bullet, Bullet, bool> BulletBulletCollisionData =
        new() { { _bullet1, _bullet2, false } };

    public static readonly TheoryData<Enemy, EnemyBullet, bool> EnemyEnemyBulletCollisionData =
        new() { { _enemy1, _enemyBullet1, false } };

    public static readonly TheoryData<Enemy, Enemy, bool> EnemyEnemyCollisionData =
        new() { { _enemy1, _enemy2, false } };

    public static readonly TheoryData<EnemyBullet, EnemyBullet, bool> EnemyBulletEnemyBulletCollisionData =
        new() { { _enemyBullet1, _enemyBullet2, false } };

    public static readonly TheoryData<Player, Enemy, bool> PlayerEnemyCollisionData =
        new() { { _player, _enemy1, true } };

    public static readonly TheoryData<Player, EnemyBullet, bool> PlayerEnemyBulletCollisionData =
        new() { { _player, _enemyBullet1, true } };

    public static readonly TheoryData<Bullet, Enemy, bool> BulletEnemyCollisionData =
        new() { { _bullet1, _enemy1, true } };

    public static readonly TheoryData<Bullet, EnemyBullet, bool> BulletEnemyBulletCollisionData =
        new() { { _bullet1, _enemyBullet1, false } };
}
