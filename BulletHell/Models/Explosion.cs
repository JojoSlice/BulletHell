using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models
{
    public class Explosion : IDisposable
    {
        public bool IsAlive { get; private set; } = true;

        public Explosion(Vector2 position, ISpriteHelper sprite)
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Dispose()
        {
        }
    }
}