using System;
using BulletHell.Constants;
using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.UI.Factories;

public class MenuUIFactory
{
    private const int ButtonWidth = 300;
    private const int ButtonHeight = 50;
    private const int InputFieldWidth = 300;
    private const int InputFieldHeight = 40;
    private const int UsernameFieldYOffset = 80;
    private const int PasswordFieldYOffset = 150;
    private const int ActionButtonYOffset = 210;
    private const int ModeToggleButtonYOffset = 270;
    private const int StartButtonYOffset = 0;
    private const int ExitButtonYOffset = 330;
    private const int LoggedInLabelYPosition = 280;
    private const int LogoutButtonYOffset = 70;
    private const int LoggedInExitButtonYOffset = 150;

    private readonly SpriteFont _font;
    private readonly Texture2D _whiteTexture;
    private readonly ITextInputHandler _textInputHandler;
    private readonly int _screenWidth;
    private readonly int _screenHeight;

    public MenuUIFactory(
        SpriteFont font,
        Texture2D whiteTexture,
        ITextInputHandler textInputHandler,
        int screenWidth,
        int screenHeight)
    {
        _font = font;
        _whiteTexture = whiteTexture;
        _textInputHandler = textInputHandler;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }

    public Button CreateModeToggleButton(string text, Action onModeToggle)
    {
        var centerX = (_screenWidth / 2) - (ButtonWidth / 2);
        var centerY = _screenHeight / 2;

        var button = new Button(
            _font,
            text,
            new Rectangle(
                centerX,
                centerY + ModeToggleButtonYOffset,
                ButtonWidth,
                ButtonHeight
            ),
            _whiteTexture
        );

        button.OnClick += onModeToggle;
        return button;
    }

    public Button CreateActionButton(string text, Action onAction)
    {
        var centerX = (_screenWidth / 2) - (ButtonWidth / 2);
        var centerY = _screenHeight / 2;

        var button = new Button(
            _font,
            text,
            new Rectangle(
                centerX,
                centerY + ActionButtonYOffset,
                ButtonWidth,
                ButtonHeight
            ),
            _whiteTexture
        );

        button.OnClick += onAction;
        return button;
    }

    public Button CreateStartGameButton(Action onStart)
    {
        var centerX = (_screenWidth / 2) - (ButtonWidth / 2);
        var centerY = _screenHeight / 2;

        var button = new Button(
            _font,
            "Starta spel",
            new(centerX, centerY + StartButtonYOffset, ButtonWidth, ButtonHeight),
            _whiteTexture
        );

        button.OnClick += onStart;
        return button;
    }

    public Button CreateExitButton(Action onExit)
    {
        var centerX = (_screenWidth / 2) - (ButtonWidth / 2);
        var centerY = _screenHeight / 2;

        var button = new Button(
            _font,
            "Avsluta",
            new(centerX, centerY + ExitButtonYOffset, ButtonWidth, ButtonHeight),
            _whiteTexture
        );

        button.OnClick += onExit;
        return button;
    }

    public InputField CreateUsernameField()
    {
        var centerX = (_screenWidth / 2) - (InputFieldWidth / 2);
        var centerY = _screenHeight / 2;

        return new InputField(
            _font,
            "Användarnamn:",
            new(centerX, centerY + UsernameFieldYOffset, InputFieldWidth, InputFieldHeight),
            _textInputHandler,
            _whiteTexture,
            isPassword: false
        );
    }

    public InputField CreatePasswordField()
    {
        var centerX = (_screenWidth / 2) - (InputFieldWidth / 2);
        var centerY = _screenHeight / 2;

        return new InputField(
            _font,
            "Lösenord:",
            new(centerX, centerY + PasswordFieldYOffset, InputFieldWidth, InputFieldHeight),
            _textInputHandler,
            _whiteTexture,
            isPassword: true
        );
    }

    public Button CreateLogoutButton(Action onLogout)
    {
        var centerX = (_screenWidth / 2) - (ButtonWidth / 2);
        var centerY = _screenHeight / 2;

        var button = new Button(
            _font,
            "Logga ut",
            new(centerX, centerY + LogoutButtonYOffset, ButtonWidth, ButtonHeight),
            _whiteTexture
        );

        button.OnClick += onLogout;
        return button;
    }

    public Vector2 GetCenteredPosition(string text, float y)
    {
        var size = _font.MeasureString(text);
        return new Vector2(((float)_screenWidth / 2) - (size.X / 2), y);
    }

    public Vector2 GetLoggedInLabelPosition(string text)
    {
        return GetCenteredPosition(text, LoggedInLabelYPosition);
    }
}
