using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Scenes;

public abstract class Scene : IDisposable
{
    protected Game1 _game;
    protected bool _disposed;

    protected Scene(Game1 game)
    {
        _game = game;
    }

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void OnEnter() { }

    public virtual void OnExit() { }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _game = null!;
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
