using System;
using Microsoft.Xna.Framework;

namespace BulletHell.Models;

public class Collider(Vector2 pos, Type? colliderType)
{
    public float Radius { get; set; }
    public Vector2 Position { get; set; } = pos;
    public readonly Type? ColliderType = colliderType;
}
