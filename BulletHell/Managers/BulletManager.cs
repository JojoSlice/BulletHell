using System;
using System.Collections.Generic;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Managers;

/// <summary>
/// Manages all bullets in the game using object pooling
/// </summary>
public class BulletManager : IBulletManager, IDisposable
{
    private readonly List<Bullet> _activeBullets = [];
    private readonly ObjectPool<Bullet> _bulletPool;
    private Texture2D? _bulletTexture;
    private bool _disposed;

    public BulletManager()
    {
        _bulletPool = new ObjectPool<Bullet>(
            createFunc: () =>
            {
                ISpriteHelper sprite = new SpriteHelper();
                return new Bullet(Vector2.Zero, Vector2.Zero, sprite);
            },
            resetAction: null, // Bullets are reset manually via Reset() method
            initialSize: 50, // Pre-allocate 50 bullets
            maxSize: 200 // Max 200 bullets in pool
        );
    }

    /// <summary>
    /// Loads the bullet texture for all bullets
    /// </summary>
    public void LoadContent(Texture2D bulletTexture)
    {
        ArgumentNullException.ThrowIfNull(bulletTexture);
        _bulletTexture = bulletTexture;
    }

    /// <summary>
    /// Creates a new bullet at the specified position and direction
    /// </summary>
    public void CreateBullet(Vector2 position, Vector2 direction)
    {
        if (_bulletTexture == null)
            throw new InvalidOperationException(
                "LoadContent must be called before creating bullets"
            );

        var bullet = _bulletPool.Get();

        bullet.Reset(position, direction);

        if (bullet.Width == 0 || bullet.Height == 0)
        {
            bullet.LoadContent(_bulletTexture);
        }

        _activeBullets.Add(bullet);
    }

    /// <summary>
    /// Updates all active bullets and removes dead/off-screen bullets
    /// </summary>
    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        foreach (var bullet in _activeBullets)
        {
            bullet.Update(gameTime);
        }

        for (int i = _activeBullets.Count - 1; i >= 0; i--)
        {
            var bullet = _activeBullets[i];
            if (bullet.ShouldBeRemoved(screenWidth, screenHeight))
            {
                _activeBullets.RemoveAt(i);
                _bulletPool.Return(bullet);
            }
        }
    }

    /// <summary>
    /// Draws all active bullets
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        foreach (var bullet in _activeBullets)
        {
            bullet.Draw(spriteBatch);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Return all active bullets to pool
                foreach (var bullet in _activeBullets)
                {
                    _bulletPool.Return(bullet);
                }
                _activeBullets.Clear();

                _bulletPool.Clear();

                // Note: _bulletTexture is managed by ContentManager, don't dispose here
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
