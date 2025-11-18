using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BulletHell.Interfaces;
using BulletHell.Models;

namespace BulletHell.Managers
{
    public class EnemyBulletManager : IEnemyBulletManager
    {
        private readonly List<EnemyBullet> _bullets = new();

        public IReadOnlyList<EnemyBullet> Bullets => _bullets;

        public void AddBullet(EnemyBullet bullet)
        {
            _bullets.Add(bullet);
        }

        public void Update(GameTime gameTime, int screenWidth, int screenHeight)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}