using System;
using System.Collections.Generic;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Managers;

public class EnemyBulletManager : IEnemyBulletManager, IDisposable
{
    private readonly List<EnemyBullet> _activeBullets = new();
    private readonly ObjectPool<EnemyBullet> _bulletPool;
    private Texture2D? _bulletTexture;

    public IReadOnlyList<EnemyBullet> Bullets => _activeBullets;

    public EnemyBulletManager()
    {
        _bulletPool = new ObjectPool<EnemyBullet>(
            createFunc: () =>
            {
                ISpriteHelper sprite = new SpriteHelper();
                return new EnemyBullet(Vector2.Zero, Vector2.Zero, sprite);
            },
            resetAction: null,
            initialSize: 100,
            maxSize: 500
        );
    }

    public void LoadContent(Texture2D texture)
    {
        _bulletTexture = texture;
    }

    public void CreateBullet(Vector2 position, Vector2 velocity)
    {
        var bullet = _bulletPool.Get();
        bullet.Reset(position, velocity);

        if (bullet.Width == 0 && _bulletTexture != null)
        {
            bullet.LoadContent(_bulletTexture);
        }

        _activeBullets.Add(bullet);
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        foreach (var bullet in _activeBullets)
        {
            bullet.Update(gameTime);
        }

        var bulletsToRemove = _activeBullets.FindAll(b => b.ShouldBeRemoved(screenWidth, screenHeight));
        foreach (var bullet in bulletsToRemove)
        {
            _activeBullets.Remove(bullet);
            _bulletPool.Return(bullet);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var bullet in _activeBullets)
        {
            bullet.Draw(spriteBatch);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bulletPool?.Dispose();
        }
    }
}
