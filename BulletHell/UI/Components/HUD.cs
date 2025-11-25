using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BulletHell.UI.Components;

public class HUD
{
    public string Message { get; set; }
    private int _hp;
    private int _Ammo;
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            Message = $"HP: {value}";
        }
    }

    public int Ammo
    {
        get => _Ammo;
        set
        {
            _Ammo = value;
            Message = $"Ammo: {value}";
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (Message == null)
            return;

        spriteBatch.DrawString(font, Message, new Vector2(10, 10), Color.White);
    }
}