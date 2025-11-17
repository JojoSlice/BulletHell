using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Scenes;

public class MenuScene(Game1 game) : Scene(game)
{
    private readonly SpriteFont font = game.Content.Load<SpriteFont>("Font");
    private readonly string title = "Bullet Hell";
    private readonly string startText = "Press ENTER to play";
    private readonly string exitText = "Press ESC to quit";
    private readonly int screenWidth = game.GraphicsDevice.Viewport.Width;
    private readonly int screenHeight = game.GraphicsDevice.Viewport.Height;

    public override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Enter))
        {
            _game.ChangeScene("Battle");
        }
        if (keyState.IsKeyDown(Keys.Escape))
        {
            _game.Exit();
        }
    }

    private Vector2 GetCenteredPosition(string text, float y)
    {
        Vector2 size = font.MeasureString(text);
        return new Vector2(screenWidth / 2 - size.X / 2, y);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, title, GetCenteredPosition(title, 200), Color.Black);
        spriteBatch.DrawString(
            font,
            startText,
            GetCenteredPosition(startText, screenHeight / 2),
            Color.Yellow
        );
        spriteBatch.DrawString(
            font,
            exitText,
            GetCenteredPosition(exitText, screenHeight / 2 + 30),
            Color.Gray
        );
    }

    public override void OnEnter()
    {
        System.Console.WriteLine("MenuScene loading");
    }
}
