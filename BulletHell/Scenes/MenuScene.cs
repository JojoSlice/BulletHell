using System;
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

    private readonly SpriteFont _font;
    private readonly Texture2D _whiteTexture;
    private readonly string _title = "Bullet Hell";
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly IMenuInputProvider _inputProvider;
    private readonly ITextInputHandler _textInputHandler;

    private Button[] _menuButtons;
    private InputField[] _menuInputs;
    private InputField? _usernameField;
    private InputField? _passwordField;
    private IMenuNavigator? _menuNavigator;

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
        _whiteTexture = whiteTexture;
        _inputProvider = inputProvider;
        _textInputHandler = textInputHandler;
        _font = game.Content.Load<SpriteFont>("Font");
        _screenWidth = game.GraphicsDevice.Viewport.Width;
        _screenHeight = game.GraphicsDevice.Viewport.Height;

        _menuButtons = [];
        _menuInputs = [];
    }

    private Vector2 GetCenteredPosition(string text, float y)
    {
        var size = _font.MeasureString(text);
        return new Vector2((_screenWidth / 2) - (size.X / 2), y);
    }

    private Button[] GetMenuButtons()
    {
        var centerX = (_screenWidth / 2) - (ButtonWidth / 2);
        var centerY = _screenHeight / 2;

        var startButton = new Button(
            _font,
            "Start Game",
            new(centerX, centerY + StartButtonYOffset, ButtonWidth, ButtonHeight),
            _whiteTexture
        );
        startButton.OnClick += () => _game.ChangeScene(SceneNames.Battle);

        var loginButton = new Button(
            _font,
            "Log In",
            new(centerX, centerY + LoginButtonYOffset, ButtonWidth, ButtonHeight),
            _whiteTexture
        );

        var exitButton = new Button(
            _font,
            "Exit",
            new(centerX, centerY + ExitButtonYOffset, ButtonWidth, ButtonHeight),
            _whiteTexture
        );
        exitButton.OnClick += () => _game.Exit();

        return [startButton, loginButton, exitButton];
    }

    private InputField[] GetMenuInputs()
    {
        var centerX = (_screenWidth / 2) - (InputFieldWidth / 2);
        var centerY = _screenHeight / 2;

        _usernameField = new InputField(
            _font,
            "Username:",
            new(centerX, centerY + UsernameFieldYOffset, InputFieldWidth, InputFieldHeight),
            _textInputHandler,
            _whiteTexture,
            isPassword: false
        );

        _passwordField = new InputField(
            _font,
            "Password:",
            new(centerX, centerY + PasswordFieldYOffset, InputFieldWidth, InputFieldHeight),
            _textInputHandler,
            _whiteTexture,
            isPassword: true
        );

        return [_usernameField, _passwordField];
    }

    public override void OnEnter()
    {
        if (_menuButtons == null || _menuButtons.Length == 0)
        {
            _menuButtons = GetMenuButtons();
            _menuInputs = GetMenuInputs();
            _menuNavigator = new MenuNavigator();

            _menuNavigator.AddItem(_menuButtons[0]); // Start button
            _menuNavigator.AddItem(_menuInputs[0]); // Username field
            _menuNavigator.AddItem(_menuInputs[1]); // Password field
            _menuNavigator.AddItem(_menuButtons[1]); // Login button
            _menuNavigator.AddItem(_menuButtons[2]); // Exit button
        }

        if (_usernameField != null)
        {
            // TODO: InputField doesn't have a Clear() method, would need to add one
            // For now, text persists between scene changes
        }
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = _inputProvider.GetMouseState();
        var keyState = _inputProvider.GetKeyboardState();

        _usernameField?.Update(gameTime, mouseState);
        _passwordField?.Update(gameTime, mouseState);

        foreach (var button in _menuButtons)
        {
            button.Update(mouseState);
        }

        if (_usernameField?.IsFocused == false && _passwordField?.IsFocused == false)
        {
            _menuNavigator?.Update(keyState);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _font,
            _title,
            GetCenteredPosition(_title, TitleYPosition),
            Color.Black
        );

        foreach (var inputField in _menuInputs)
        {
            inputField.Draw(spriteBatch);
        }

        foreach (var button in _menuButtons)
        {
            button.Draw(spriteBatch);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Note: We don't dispose _whiteTexture here as it's a shared resource
            // owned by Game1 and will be disposed there

            if (_menuButtons != null)
            {
                foreach (var button in _menuButtons)
                {
                    button.Dispose();
                }
            }

            if (_menuInputs != null)
            {
                foreach (var inputField in _menuInputs)
                {
                    inputField.Dispose();
                }
            }

            _menuNavigator = null;
            _usernameField = null;
            _passwordField = null;
        }

        base.Dispose(disposing);
    }
}
