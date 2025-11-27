using System;
using System.Net.Http;
using System.Threading.Tasks;
using BulletHell.Constants;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.Services;
using BulletHell.Services.Validation;
using BulletHell.UI.Components;
using BulletHell.UI.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BulletHell.Scenes;

public class MenuScene : Scene
{
    private const int TitleYPosition = 200;
    private const int FeedbackMessageYPosition = 50;
    private const float FeedbackDuration = 3f;

    private readonly SpriteFont _font;
    private readonly string _title = "Bullet Hell";
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly IMenuInputProvider _inputProvider;
    private readonly IUserApiClient _userApiClient;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserCredentialsValidator _validator;
    private readonly FeedbackMessageDisplay _feedbackDisplay;
    private readonly MenuUIFactory _uiFactory;

    private Button[] _menuButtons;
    private InputField? _usernameField;
    private InputField? _passwordField;
    private IMenuNavigator? _menuNavigator;
    private Button _modeToggleButton = null!;
    private Button _actionButton = null!;

    private Texture2D? _backgroundTexture;
    private Song? _menuMusic;

    private RegistrationMode _currentMode = RegistrationMode.Login;

    public RegistrationMode CurrentMode => _currentMode;

    public MenuScene(Game1 game, Texture2D whiteTexture)
        : this(
            game,
            whiteTexture,
            new MouseKeyboardInputProvider(),
            new KeyboardTextInputHandler(),
            new UserApiClient(new HttpClient { BaseAddress = new Uri("http://localhost:5000") }),
            new BCryptPasswordHasher()
        ) { }

    public MenuScene(
        Game1 game,
        Texture2D whiteTexture,
        IMenuInputProvider inputProvider,
        ITextInputHandler textInputHandler,
        IUserApiClient userApiClient,
        IPasswordHasher passwordHasher
    )
        : base(game)
    {
        _inputProvider = inputProvider;
        _font = game.Content.Load<SpriteFont>("Font");
        _screenWidth = game.GraphicsDevice.Viewport.Width;
        _screenHeight = game.GraphicsDevice.Viewport.Height;
        _userApiClient = userApiClient;
        _passwordHasher = passwordHasher;
        _validator = new UserCredentialsValidator();
        _feedbackDisplay = new FeedbackMessageDisplay();
        _uiFactory = new MenuUIFactory(_font, whiteTexture, textInputHandler, _screenWidth, _screenHeight);
        _menuButtons = [];
    }

    // Constructor for testing - doesn't require Game1.Content or GraphicsDevice
    internal MenuScene(
        Game1 game,
        Texture2D whiteTexture,
        SpriteFont font,
        int screenWidth,
        int screenHeight,
        IMenuInputProvider inputProvider,
        ITextInputHandler textInputHandler,
        IUserApiClient userApiClient,
        IPasswordHasher passwordHasher
    )
        : base(game)
    {
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _inputProvider = inputProvider;
        _userApiClient = userApiClient;
        _passwordHasher = passwordHasher;
        _validator = new UserCredentialsValidator();
        _feedbackDisplay = new FeedbackMessageDisplay();
        _uiFactory = new MenuUIFactory(font, whiteTexture, textInputHandler, screenWidth, screenHeight);
        _menuButtons = [];
    }

    public void ToggleMode()
    {
        _currentMode =
            _currentMode == RegistrationMode.Login
                ? RegistrationMode.Register
                : RegistrationMode.Login;

        // Uppdatera knapp-text
        UpdateActionButtonText();
    }

    public string GetActionButtonText()
    {
        return _currentMode == RegistrationMode.Login ? "Log In" : "Register";
    }

    private void UpdateActionButtonText()
    {
        _actionButton?.UpdateText(GetActionButtonText());
    }

    private void OnModeToggleClicked()
    {
        ToggleMode();
        _modeToggleButton.UpdateText($"Mode: {_currentMode}");
    }

    public async Task<RegistrationResult> RegisterUserAsync(string username, string password)
    {
        var validationResult = _validator.Validate(username, password);
        if (!validationResult.IsValid)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = validationResult.ErrorMessage,
            };
        }

        var passwordHash = _passwordHasher.HashPassword(password);
        return await _userApiClient.RegisterUserAsync(username, passwordHash);
    }

    private async void OnActionButtonClicked()
    {
        if (_currentMode == RegistrationMode.Register)
        {
            await HandleRegistrationAsync();
        }
        else
        {
            HandleLogin();
        }
    }

    private async Task HandleRegistrationAsync()
    {
        var username = _usernameField?.Text ?? "";
        var password = _passwordField?.Text ?? "";

        var originalText = _actionButton.Text;
        _actionButton.UpdateText("Registrerar...");
        _actionButton.Enabled = false;

        try
        {
            var result = await RegisterUserAsync(username, password);

            if (result.Success)
            {
                ShowMessage($"Välkommen {username}!", Color.Green);

                // Rensa fält (note: InputField doesn't have Clear() method yet, would need to be added)
                // För nu hoppar vi över detta

                ToggleMode();
            }
            else
            {
                ShowMessage(result.Message, Color.Red);
            }
        }
        finally
        {
            _actionButton.UpdateText(originalText);
            _actionButton.Enabled = true;
        }
    }

    private async void HandleLogin()
    {
        var username = _usernameField?.Text ?? "";
        var password = _passwordField?.Text ?? "";

        var validationResult = _validator.Validate(username, password);
        if (!validationResult.IsValid)
        {
            ShowMessage(validationResult.ErrorMessage, Color.Red);
            return;
        }

        var originalText = _actionButton.Text;
        _actionButton.UpdateText("Loggar in...");
        _actionButton.Enabled = false;

        try
        {
            var passwordHash = _passwordHasher.HashPassword(password);
            var result = await _userApiClient.LoginAsync(username, passwordHash);

            if (result.Success)
            {
                ShowMessage($"Välkommen tillbaka {username}!", Color.Green);
                // TODO: Store user session (UserId, Username)
                // TODO: Optionally transition to game
            }
            else
            {
                ShowMessage(result.Message, Color.Red);
            }
        }
        finally
        {
            _actionButton.UpdateText(originalText);
            _actionButton.Enabled = true;
        }
    }

    private void ShowMessage(string message, Color color)
    {
        _feedbackDisplay.Show(message, color, FeedbackDuration);
    }

    public override void OnEnter()
    {
        _backgroundTexture = _game.Content.Load<Texture2D>("bullethellmenu");

        try
        {
            _menuMusic = _game.Content.Load<Song>("Project_137");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(_menuMusic);
        }
        catch (Microsoft.Xna.Framework.Audio.NoAudioHardwareException)
        {
            _menuMusic = null;
        }

        InitializeModeToggleButton();
        InitializeActionButton();
        InitializeMenuComponents();
        SetupMenuNavigation();
    }

    private void InitializeModeToggleButton()
    {
        _modeToggleButton = _uiFactory.CreateModeToggleButton(_currentMode, OnModeToggleClicked);
    }

    private void InitializeActionButton()
    {
        _actionButton = _uiFactory.CreateActionButton(GetActionButtonText(), OnActionButtonClicked);
    }

    private void InitializeMenuComponents()
    {
        if (_menuButtons == null || _menuButtons.Length == 0)
        {
            var startButton = _uiFactory.CreateStartGameButton(() => _game.ChangeScene(SceneNames.Battle));
            var exitButton = _uiFactory.CreateExitButton(() => _game.Exit());
            _menuButtons = [startButton, exitButton];

            _usernameField = _uiFactory.CreateUsernameField();
            _passwordField = _uiFactory.CreatePasswordField();
        }
    }

    private void SetupMenuNavigation()
    {
        if (_menuNavigator == null)
        {
            _menuNavigator = new MenuNavigator();
            _menuNavigator.AddItem(_menuButtons[0]); // Start button
            _menuNavigator.AddItem(_usernameField!); // Username field
            _menuNavigator.AddItem(_passwordField!); // Password field
            _menuNavigator.AddItem(_actionButton); // Action button (Login/Register)
            _menuNavigator.AddItem(_menuButtons[1]); // Exit button
        }
    }

    public override void OnExit()
    {
        if (_menuMusic != null)
        {
            MediaPlayer.Stop();
        }

        if (_modeToggleButton != null)
        {
            _modeToggleButton.OnClick -= OnModeToggleClicked;
        }

        if (_actionButton != null)
        {
            _actionButton.OnClick -= OnActionButtonClicked;
        }

        base.OnExit();
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = _inputProvider.GetMouseState();
        var keyState = _inputProvider.GetKeyboardState();

        _usernameField?.Update(gameTime, mouseState);
        _passwordField?.Update(gameTime, mouseState);

        _modeToggleButton?.Update(mouseState);
        _actionButton?.Update(mouseState);

        foreach (var button in _menuButtons)
        {
            button.Update(mouseState);
        }

        if (_usernameField?.IsFocused == false && _passwordField?.IsFocused == false)
        {
            _menuNavigator?.Update(keyState);
        }

        _feedbackDisplay.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw background with aspect ratio preserved
        if (_backgroundTexture != null)
        {
            float scaleX = (float)_screenWidth / _backgroundTexture.Width;
            float scaleY = (float)_screenHeight / _backgroundTexture.Height;
            float scale = Math.Min(scaleX, scaleY);

            int drawWidth = (int)(_backgroundTexture.Width * scale);
            int drawHeight = (int)(_backgroundTexture.Height * scale);
            int drawX = (_screenWidth - drawWidth) / 2;
            int drawY = (_screenHeight - drawHeight) / 2;

            spriteBatch.Draw(
                _backgroundTexture,
                new Rectangle(drawX, drawY, drawWidth, drawHeight),
                Color.White
            );
        }

        spriteBatch.DrawString(
            _font,
            _title,
            _uiFactory.GetCenteredPosition(_title, TitleYPosition),
            Color.Black
        );

        _modeToggleButton?.Draw(spriteBatch);

        _usernameField?.Draw(spriteBatch);
        _passwordField?.Draw(spriteBatch);

        _actionButton?.Draw(spriteBatch);

        foreach (var button in _menuButtons)
        {
            button.Draw(spriteBatch);
        }

        _feedbackDisplay.Draw(spriteBatch, _font, _screenWidth, FeedbackMessageYPosition);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _modeToggleButton?.Dispose();
            _actionButton?.Dispose();

            if (_menuButtons != null)
            {
                foreach (var button in _menuButtons)
                {
                    button.Dispose();
                }
            }

            _usernameField?.Dispose();
            _passwordField?.Dispose();

            _menuNavigator = null;
            _usernameField = null;
            _passwordField = null;
        }

        base.Dispose(disposing);
    }
}