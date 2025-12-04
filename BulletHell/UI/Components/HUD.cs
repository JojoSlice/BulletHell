using BulletHell.Configurations;
using BulletHell.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BulletHell.UI.Components;

public class HUD
{
    public int HP { get; set; }
    public int MaxHP { get; set; } = 100;

    public int Lives { get; set; } = PlayerConfig.Lives;
    public Texture2D? LifeTexture { get; set; }
    public int Score { get; private set; }

    public void UpdateLives(int lives)
    {
        Lives = lives;
    }

    public void UpdateScore(int value)
    {
        Score = value;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // HP bar bakgrund
        spriteBatch.Draw(TextureHelper.WhitePixel(spriteBatch.GraphicsDevice), new Rectangle(10, 10, 200, 20),
            Color.DarkRed);

        // HP bar fylld
        int hpWidth = (int)(200 * (HP / (float)MaxHP));
        spriteBatch.Draw(TextureHelper.WhitePixel(spriteBatch.GraphicsDevice), new Rectangle(10, 10, hpWidth, 20),
            Color.LimeGreen);

        // Draw lives below HP bar
        if (LifeTexture != null)
        {
            for (int i = 0; i < Lives; i++)
            {
                spriteBatch.Draw(
                    LifeTexture,
                    new Vector2(10f + i * (LifeTexture.Width + 5f), 40f),
                    Color.White
                );
            }
        }
        // Draw Score
        spriteBatch.DrawString(
            TextureHelper.DefaultFont,
            $"Score: {Score}",
            new Vector2(10f, 70f),
            Color.White
        );
    }
}