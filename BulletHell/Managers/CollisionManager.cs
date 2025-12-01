using BulletHell.Models;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BulletHell.Managers;

public class CollisionManager(Player player, BulletManager bm, EnemyManager em, EnemyBulletManager ebm)
{
    public void CheckCollisions()
    {
        CheckPlayerCollisions();
        foreach (var bullet in bm.Bullets)
        {
            foreach (var enemy in em.Enemies)
            {
                if (bullet.Collider.Distance(enemy.Collider) > 10)
                {
                    continue;
                }

                if (bullet.Collider.IsCollidingWith(enemy.Collider))
                {
                    bullet.HitEnemy();
                }
            }
        }
    }
    private void CheckPlayerCollisions()
    {
        foreach (var enemyBullet in ebm.Bullets)
        {
            if (enemyBullet.Collider.Distance(player.Collider) > 10)
            {
                continue;
            }
            if (enemyBullet.Collider.IsCollidingWith(player.Collider))
            {
                enemyBullet.HitPlayer();
            }
        }

        foreach (var enemy in em.Enemies.Where(enemy =>
                     player.Collider.IsCollidingWith(enemy.Collider)))
        {
                player.TakeDamage();
                PushBack(enemy.Collider.Position);
        }
    }

    private void PushBack(Vector2 enemyPosition)
    {
        var direction = player.Collider.Position - enemyPosition;
        if (direction == Vector2.Zero)
        {
            direction = -Vector2.UnitY;
        }

        // Force is in pixels per second; duration determines how long the knockback lasts.
        const float knockbackForce = 300f;
        const float knockbackDuration = 0.18f;

        player.ApplyKnockback(direction, knockbackForce, knockbackDuration);
    }
}