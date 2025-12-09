using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models
{
    public class Explosion : IDisposable
    {
        private readonly ISpriteHelper _sprite;
        private readonly Vector2 _position;
        public bool IsAlive { get; private set; } = true;

        public Explosion(Vector2 position, ISpriteHelper sprite)
        {
            _sprite = sprite;
            _position = position;
        }

        public void LoadContent(Texture2D texture)
        {
            _sprite.LoadSpriteSheet(texture, frameWidth: 32, frameHeight: 32, animationSpeed: 0.05f);
        }

        public void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);

            if (_sprite.IsAnimationFinished)
                IsAlive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, _position, 0f, 1f); // utan test
        }

        public void Dispose()
        {
            _sprite.Dispose();
        }
    }
}