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

    /// <summary>
    /// Gets the camera transformation matrix for this scene.
    /// Returns null if the scene doesn't use a camera.
    /// </summary>
    public virtual Matrix? GetCameraTransform() => null;

    /// <summary>
    /// Draws HUD elements that should not be affected by camera transformation.
    /// These elements use screen coordinates and remain fixed on screen.
    /// </summary>
    public virtual void DrawHUD(SpriteBatch spriteBatch) { }

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
