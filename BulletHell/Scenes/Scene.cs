using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Scenes;

public abstract class Scene : IDisposable
{
    protected Game1 _game;
    protected IServiceScope? _serviceScope;
    protected IServiceProvider? _scopeServices;
    protected bool _disposed;

    protected Scene(Game1 game)
    {
        _game = game;
    }

    /// <summary>
    /// Initializes the service scope for this scene.
    /// This should be called after the scene is created to set up dependency injection.
    /// </summary>
    public virtual void InitializeScope(IServiceProvider serviceProvider)
    {
        _serviceScope = serviceProvider.CreateScope();
        _scopeServices = _serviceScope.ServiceProvider;
    }

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(SpriteBatch spriteBatch);

    /// <summary>
    /// Gets the camera transformation matrix for this scene.
    /// Returns null if the scene doesn't use a camera.
    /// </summary>
    public virtual Matrix? GetCameraTransform() => null;

    /// <summary>
    /// Draws background elements that should not be affected by camera transformation.
    /// These are rendered before the world objects.
    /// </summary>
    public virtual void DrawBackground(SpriteBatch spriteBatch) { }

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
                // Dispose service scope first
                _serviceScope?.Dispose();
                _serviceScope = null;
                _scopeServices = null;

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
