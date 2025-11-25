using BulletHell.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BulletHell.UI.Components;

public class HUD
{
    public int HP { get; set; }
    public int MaxHP { get; set; } = 100;

    public void Draw(SpriteBatch spriteBatch)
    {
        // HP bar bakgrund
        spriteBatch.Draw(TextureHelper.WhitePixel(spriteBatch.GraphicsDevice), new Rectangle(10, 10, 200, 20), Color.DarkRed);

        // HP bar fylld
        int hpWidth = (int)(200 * (HP / (float)MaxHP));
        spriteBatch.Draw(TextureHelper.WhitePixel(spriteBatch.GraphicsDevice), new Rectangle(10, 10, hpWidth, 20), Color.LimeGreen);
    }
}