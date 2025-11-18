using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Scenes;

public class MenuScene(Game1 game) : Scene(game)
{
    private readonly SpriteFont font = game.Content.Load<SpriteFont>("Font");
    private readonly string title = "Bullet Hell";
    private readonly int screenWidth = game.GraphicsDevice.Viewport.Width;
    private readonly int screenHeight = game.GraphicsDevice.Viewport.Height;
    private Button[] menuButtons = [];
    private InputField[] menuInputs = [];
    private InputField usernameField = null!;
    private InputField passwordField = null!;
    private IMenuNavigator menuNavigator = null!;

    private Vector2 GetCenteredPosition(string text, float y)
    {
        Vector2 size = font.MeasureString(text);
        return new Vector2(screenWidth / 2 - size.X / 2, y);
    }

    private Button[] GetMenuButtons()
    {
        var startButton = new Button(
            _game.GraphicsDevice,
            font,
            "Start Game",
            new(screenWidth / 2 - 150, screenHeight / 2, 300, 50)
        );
        startButton.OnClick += () => _game.ChangeScene("Battle");

        var loginButton = new Button(
            _game.GraphicsDevice,
            font,
            "Log In",
            new(screenWidth / 2 - 150, screenHeight / 2 + 220, 300, 50)
        );

        var exitButton = new Button(
            _game.GraphicsDevice,
            font,
            "Exit",
            new(screenWidth / 2 - 150, screenHeight / 2 + 300, 300, 50)
        );
        exitButton.OnClick += () => _game.Exit();

        return [startButton, loginButton, exitButton];
    }

    private InputField[] GetMenuInputs()
    {
        return
        [
            usernameField = new InputField(
                _game.GraphicsDevice,
                font,
                "Username:",
                new(screenWidth / 2 - 150, screenHeight / 2 + 80, 300, 40),
                isPassword: false
            ),
            passwordField = new InputField(
                _game.GraphicsDevice,
                font,
                "Password:",
                new(screenWidth / 2 - 150, screenHeight / 2 + 150, 300, 40),
                isPassword: true
            ),
        ];
    }

    public override void OnEnter()
    {
        System.Console.WriteLine("MenuScene loading");
        menuButtons = GetMenuButtons();
        menuInputs = GetMenuInputs();

        menuNavigator = new MenuNavigator();

        menuNavigator.AddItem(menuButtons[0]); // Start button
        menuNavigator.AddItem(usernameField);
        menuNavigator.AddItem(passwordField);
        menuNavigator.AddItem(menuButtons[1]); // Login button
        menuNavigator.AddItem(menuButtons[2]); // Exit button
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        var keyState = Keyboard.GetState();

        usernameField.Update(gameTime, mouseState);
        passwordField.Update(gameTime, mouseState);

        foreach (var button in menuButtons)
        {
            button.Update(mouseState);
        }

        if (!usernameField.IsFocused && !passwordField.IsFocused)
        {
            menuNavigator.Update(keyState);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, title, GetCenteredPosition(title, 200), Color.Black);

        foreach (var inputField in menuInputs)
        {
            inputField.Draw(spriteBatch);
        }

        foreach (var button in menuButtons)
        {
            button.Draw(spriteBatch);
        }
    }
}
