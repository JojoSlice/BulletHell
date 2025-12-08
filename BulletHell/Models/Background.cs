using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models
{
    internal class Background : IDisposable
    {
        private readonly ISpriteHelper _sprite;
        private bool _disposed;

        public Background(ISpriteHelper sprite)
        {
            _sprite = sprite;
        }

        public void LoadContent(Texture2D bgTexture)
        {
            ArgumentNullException.ThrowIfNull(bgTexture);

            // Frames are 1024x1024
            _sprite.LoadSpriteSheet(
                bgTexture,
                1024,
                1024,
                SpriteDefaults.AnimationSpeed
            );
        }

        public void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            _sprite.Draw(spriteBatch, Vector2.Zero, 0f, 2f);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _sprite?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
