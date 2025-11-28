using BulletHell.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.DebugUI;

public class DebugOverlay
{
    public bool IsVisible { get; private set; } = true;

    private readonly SpriteFont _font;
    private readonly Texture2D _bgTexture;

    public DebugOverlay(SpriteFont font, Texture2D bgTexture)
    {
        _font = font;
        _bgTexture = bgTexture;
    }

    public void Toggle() => IsVisible = !IsVisible;

    public void Draw(SpriteBatch spriteBatch, BattleScene scene)
    {
        if (!IsVisible) return;

    }
}