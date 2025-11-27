using System;
using Microsoft.Xna.Framework;

namespace BulletHell.Models;

public class Collider(Vector2 position, Type? colliderType, float radius = 0f)
{
    public float Radius { get; } = radius;
    public Vector2 Position { get; set; } = position;
    public readonly Type? ColliderType = colliderType;
}
