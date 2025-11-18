using System.Collections.Generic;
using BulletHell.Models;
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
    private List<Button> menuButtons = [];

    private Vector2 GetCenteredPosition(string text, float y)
    {
        Vector2 size = font.MeasureString(text);
        return new Vector2(screenWidth / 2 - size.X / 2, y);
    }

    private List<string> GetMenuText() => [startText, exitText];

    private List<Button> GetMenuButtons()
    {
        int padding = 0;
        List<Button> buttons = [];
        var menuText = GetMenuText();

        foreach (var text in menuText)
        {
            var button = new Button(
                _game.GraphicsDevice,
                font,
                text,
                new(screenWidth / 2, screenHeight / 2 - padding, 200, 50)
            );

            padding += 60;

            buttons.Add(button);
        }

        return buttons;
    }

    public override void OnEnter()
    {
        System.Console.WriteLine("MenuScene loading");
        menuButtons = GetMenuButtons();
    }

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

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, title, GetCenteredPosition(title, 200), Color.Black);
        foreach (var button in menuButtons)
        {
            button.Draw(spriteBatch);
        }
    }
}
