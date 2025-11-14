using System;
using System.Collections.Generic;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Managers;

public class BulletManager : IBulletManager
{
    private readonly List<Bullet> _bullets = [];
    private Texture2D? _bulletTexture;

    public void LoadContent(Texture2D bulletTexture)
    {
        ArgumentNullException.ThrowIfNull(bulletTexture);
        _bulletTexture = bulletTexture;
    }

    public void CreateBullet(Vector2 position, Vector2 direction)
    {
        if (_bulletTexture == null)
            throw new InvalidOperationException(
                "LoadContent must be called before creating bullets"
            );

        ISpriteHelper bulletSprite = new SpriteHelper();
        var bullet = new Bullet(position, direction, bulletSprite);
        bullet.LoadContent(_bulletTexture);
        _bullets.Add(bullet);
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        foreach (var bullet in _bullets)
        {
            bullet.Update(gameTime);
        }

        _bullets.RemoveAll(b => b.ShouldBeRemoved(screenWidth, screenHeight));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        foreach (var bullet in _bullets)
        {
            bullet.Draw(spriteBatch);
        }
    }
}
