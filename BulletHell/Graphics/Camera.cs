using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Graphics;

/// <summary>
/// Camera class that follows a target and applies transformation to the game world.
/// </summary>
public class Camera
{
    private float? _worldWidth;
    private float? _worldHeight;
    private const float Zoom = 1.5f;

    /// <summary>
    /// Gets the transformation matrix for rendering.
    /// </summary>
    public Matrix Transform { get; private set; } = Matrix.Identity;

    /// <summary>
    /// Gets the current camera position in world coordinates.
    /// </summary>
    public Vector2 Position { get; private set; } = Vector2.Zero;

    /// <summary>
    /// Updates the camera to follow a target position with smooth interpolation.
    /// </summary>
    /// <param name="target">The target position to follow (usually player position).</param>
    /// <param name="viewport">The game viewport for calculating screen center.</param>
    /// <param name="smoothFactor">Interpolation factor (0.0 = no movement, 1.0 = instant). Default is 0.1.</param>
    public void Follow(Vector2 target, Viewport viewport, float smoothFactor = 0.1f)
    {
        float visibleWidth = viewport.Width / Zoom;
        float visibleHeight = viewport.Height / Zoom;

        Vector2 targetPosition = target - new Vector2(
            visibleWidth / 2f,
            visibleHeight * 0.65f
        );

        Position = Vector2.Lerp(Position, targetPosition, smoothFactor);

        if (_worldWidth.HasValue && _worldHeight.HasValue)
        {
            float maxX = Math.Max(0, _worldWidth.Value - visibleWidth);
            float maxY = Math.Max(0, _worldHeight.Value - visibleHeight);

            Position = new Vector2(
                MathHelper.Clamp(Position.X, 0, maxX),
                MathHelper.Clamp(Position.Y, 0, maxY)
            );
        }

        Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                    Matrix.CreateScale(Zoom);
    }

    /// <summary>
    /// Sets the world boundaries to prevent the camera from showing areas outside the world.
    /// </summary>
    /// <param name="width">World width in pixels.</param>
    /// <param name="height">World height in pixels.</param>
    public void SetWorldBounds(float width, float height)
    {
        _worldWidth = width;
        _worldHeight = height;
    }

    /// <summary>
    /// Clears the world boundaries, allowing the camera to follow the target without restrictions.
    /// </summary>
    public void ClearWorldBounds()
    {
        _worldWidth = null;
        _worldHeight = null;
    }
}
