using BulletHell.Constants;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Scenes;

public class MenuScene : Scene
{
    private const int ButtonWidth = 300;
    private const int ButtonHeight = 50;
    private const int InputFieldWidth = 300;
    private const int InputFieldHeight = 40;
    private const int TitleYPosition = 200;
    private const int StartButtonYOffset = 0;
    private const int UsernameFieldYOffset = 80;
    private const int PasswordFieldYOffset = 150;
    private const int LoginButtonYOffset = 220;
    private const int ExitButtonYOffset = 300;
    private const int BorderThickness = 2;

    private readonly SpriteFont font;
    private readonly Texture2D whiteTexture;
    private readonly string title = "Bullet Hell";
    private readonly int screenWidth;
    private readonly int screenHeight;
    private readonly IMenuInputProvider inputProvider;
    private readonly ITextInputHandler textInputHandler;

    private Button[] menuButtons;
    private InputField[] menuInputs;
    private InputField usernameField;
    private InputField passwordField;
    private IMenuNavigator menuNavigator;

    public MenuScene(Game1 game, Texture2D whiteTexture)
        : this(game, whiteTexture, new MouseKeyboardInputProvider(), new KeyboardTextInputHandler())
    { }

    public MenuScene(
        Game1 game,
        Texture2D whiteTexture,
        IMenuInputProvider inputProvider,
        ITextInputHandler textInputHandler
    )
        : base(game)
    {
        this.whiteTexture = whiteTexture;
        this.inputProvider = inputProvider;
        this.textInputHandler = textInputHandler;
        font = game.Content.Load<SpriteFont>("Font");
        screenWidth = game.GraphicsDevice.Viewport.Width;
        screenHeight = game.GraphicsDevice.Viewport.Height;

        menuButtons = [];
        menuInputs = [];
        usernameField = null!;
        passwordField = null!;
        menuNavigator = null!;
    }

    private Vector2 GetCenteredPosition(string text, float y)
    {
        Vector2 size = font.MeasureString(text);
        return new Vector2(screenWidth / 2 - size.X / 2, y);
    }

    private Button[] GetMenuButtons()
    {
        int centerX = screenWidth / 2 - ButtonWidth / 2;
        int centerY = screenHeight / 2;

        var startButton = new Button(
            font,
            "Start Game",
            new(centerX, centerY + StartButtonYOffset, ButtonWidth, ButtonHeight),
            whiteTexture
        );
        startButton.OnClick += () => _game.ChangeScene(SceneNames.Battle);

        var loginButton = new Button(
            font,
            "Log In",
            new(centerX, centerY + LoginButtonYOffset, ButtonWidth, ButtonHeight),
            whiteTexture
        );

        var exitButton = new Button(
            font,
            "Exit",
            new(centerX, centerY + ExitButtonYOffset, ButtonWidth, ButtonHeight),
            whiteTexture
        );
        exitButton.OnClick += () => _game.Exit();

        return [startButton, loginButton, exitButton];
    }

    private InputField[] GetMenuInputs()
    {
        int centerX = screenWidth / 2 - InputFieldWidth / 2;
        int centerY = screenHeight / 2;

        return
        [
            usernameField = new InputField(
                font,
                "Username:",
                new(centerX, centerY + UsernameFieldYOffset, InputFieldWidth, InputFieldHeight),
                textInputHandler,
                whiteTexture,
                isPassword: false
            ),
            passwordField = new InputField(
                font,
                "Password:",
                new(centerX, centerY + PasswordFieldYOffset, InputFieldWidth, InputFieldHeight),
                textInputHandler,
                whiteTexture,
                isPassword: true
            ),
        ];
    }

    public override void OnEnter()
    {
        if (menuButtons == null || menuButtons.Length == 0)
        {
            menuButtons = GetMenuButtons();
            menuInputs = GetMenuInputs();
            menuNavigator = new MenuNavigator();

            menuNavigator.AddItem(menuButtons[0]); // Start button
            menuNavigator.AddItem(menuInputs[0]); // Username field
            menuNavigator.AddItem(menuInputs[1]); // Password field
            menuNavigator.AddItem(menuButtons[1]); // Login button
            menuNavigator.AddItem(menuButtons[2]); // Exit button
        }

        if (usernameField != null)
        {
            // TODO: InputField doesn't have a Clear() method, would need to add one
            // For now, text persists between scene changes
        }
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = inputProvider.GetMouseState();
        var keyState = inputProvider.GetKeyboardState();

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
        spriteBatch.DrawString(
            font,
            title,
            GetCenteredPosition(title, TitleYPosition),
            Color.Black
        );

        foreach (var inputField in menuInputs)
        {
            inputField.Draw(spriteBatch);
        }

        foreach (var button in menuButtons)
        {
            button.Draw(spriteBatch);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Note: We don't dispose whiteTexture here as it's a shared resource
            // owned by Game1 and will be disposed there

            if (menuButtons != null)
            {
                foreach (var button in menuButtons)
                {
                    button.Dispose();
                }
            }

            if (menuInputs != null)
            {
                foreach (var inputField in menuInputs)
                {
                    inputField.Dispose();
                }
            }
        }

        base.Dispose(disposing);
    }
}
