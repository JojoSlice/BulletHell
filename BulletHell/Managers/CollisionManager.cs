using BulletHell.Configurations;
using BulletHell.Factories;
using BulletHell.Helpers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace BulletHell.Managers;

public class CollisionManager
{
    private readonly Player _player;
    private readonly BulletManager<Player> _pbm;
    private readonly EnemyManager _em;
    private readonly BulletManager<Enemy> _ebm;
    private readonly ExplosionManager _expm;
    private readonly ISpriteHelperFactory _spriteHelperFactory;

    public CollisionManager(
        Player player,
        BulletManager<Player> pbm,
        EnemyManager em,
        BulletManager<Enemy> ebm,
        ExplosionManager expm,
        ISpriteHelperFactory spriteHelperFactory
    )
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _pbm = pbm ?? throw new ArgumentNullException(nameof(pbm));
        _em = em ?? throw new ArgumentNullException(nameof(em));
        _ebm = ebm ?? throw new ArgumentNullException(nameof(ebm));
        _expm = expm ?? throw new ArgumentNullException(nameof(expm));
        _spriteHelperFactory = spriteHelperFactory ?? throw new ArgumentNullException(nameof(spriteHelperFactory));
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
                        _expm.Add(new Explosion(enemy.Position, _spriteHelperFactory.Create()));
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