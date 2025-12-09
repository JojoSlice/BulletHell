using BulletHell.Models;
using Microsoft.Xna.Framework;
using System.Linq;
using BulletHell.Configurations;
using BulletHell.Helpers;

namespace BulletHell.Managers;

public class CollisionManager
{
    private readonly Player _player;
    private readonly BulletManager<Player> _pbm;
    private readonly EnemyManager _em;
    private readonly BulletManager<Enemy> _ebm;
    private readonly ExplosionManager _expm;

    public CollisionManager(
        Player player,
        BulletManager<Player> pbm,
        EnemyManager em,
        BulletManager<Enemy> ebm,
        ExplosionManager expm
    )
    {
        _player = player;
        _pbm = pbm;
        _em = em;
        _ebm = ebm;
        _expm = expm;
    }

    public void CheckCollisions()
    {
        CheckPlayerCollisions();
        foreach (var bullet in _pbm.Bullets)
        {
            foreach (var enemy in _em.Enemies)
            {
                if (bullet.Collider.Distance(enemy.Collider) > 10)
                {
                    continue;
                }

                if (bullet.Collider.IsCollidingWith(enemy.Collider))
                {
                    var wasAlive = enemy.IsAlive;
                    enemy.TakeDamage(bullet.Damage);
                    if (wasAlive && !enemy.IsAlive)
                    {
                        _player.AddScore(EnemyConfig.ScoreValue);
                        _expm.Add(new Explosion(enemy.Position, new SpriteHelper()));
                    }

                    bullet.MarkHit();
                }
            }
        }
    }

    private void CheckPlayerCollisions()
    {
        foreach (var enemyBullet in _ebm.Bullets)
        {
            if (enemyBullet.Collider.Distance(_player.Collider) > 10)
            {
                continue;
            }

            if (enemyBullet.Collider.IsCollidingWith(_player.Collider))
            {
                _player.TakeDamage(enemyBullet.Damage);
                enemyBullet.MarkHit();
            }
        }

        foreach (var enemy in _em.Enemies.Where(enemy =>
                     _player.Collider.IsCollidingWith(enemy.Collider)))
        {
            _player.TakeDamage(enemy.CollisionDamage);
            PushBack(enemy.Collider.Position);
        }
    }

    private void PushBack(Vector2 enemyPosition)
    {
        var direction = _player.Collider.Position - enemyPosition;
        if (direction == Vector2.Zero)
        {
            direction = -Vector2.UnitY;
        }

        // Force is in pixels per second; duration determines how long the knockback lasts.
        const float knockbackForce = 300f;
        const float knockbackDuration = 0.18f;

        _player.ApplyKnockback(direction, knockbackForce, knockbackDuration);
    }
}