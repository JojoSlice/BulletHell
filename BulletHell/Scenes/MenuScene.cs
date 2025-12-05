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
    private readonly IApiClient _apiClient;
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
    private Button? _logoutButton;

    private bool _isLoggedIn = false;
    private string? _loggedInUsername;
    private int? _loggedInUserId;

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
            new ApiClient(new HttpClient { BaseAddress = new Uri("http://localhost:5111") }),
            new BCryptPasswordHasher()
        ) { }

    public MenuScene(
        Game1 game,
        Texture2D whiteTexture,
        IMenuInputProvider inputProvider,
        ITextInputHandler textInputHandler,
        IApiClient apiClient,
        IPasswordHasher passwordHasher
    )
        : base(game)
    {
        _inputProvider = inputProvider;
        _font = game.Content.Load<SpriteFont>("Font");
        _screenWidth = game.GraphicsDevice.Viewport.Width;
        _screenHeight = game.GraphicsDevice.Viewport.Height;
        _apiClient = apiClient;
        _passwordHasher = passwordHasher;
        _validator = new UserCredentialsValidator();
        _feedbackDisplay = new FeedbackMessageDisplay();
        _uiFactory = new MenuUIFactory(
            _font,
            whiteTexture,
            textInputHandler,
            _screenWidth,
            _screenHeight
        );
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
        IApiClient apiClient,
        IPasswordHasher passwordHasher
    )
        : base(game)
    {
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _inputProvider = inputProvider;
        _apiClient = apiClient;
        _passwordHasher = passwordHasher;
        _validator = new UserCredentialsValidator();
        _feedbackDisplay = new FeedbackMessageDisplay();
        _uiFactory = new MenuUIFactory(
            font,
            whiteTexture,
            textInputHandler,
            screenWidth,
            screenHeight
        );
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
        return _currentMode == RegistrationMode.Login ? "Logga in" : "Registrera";
    }

    public string GetModeToggleButtonText()
    {
        return _currentMode == RegistrationMode.Login ? "Registrera dig" : "Logga in";
    }

    private void UpdateActionButtonText()
    {
        _actionButton?.UpdateText(GetActionButtonText());
    }

    private void OnModeToggleClicked()
    {
        ToggleMode();
        _modeToggleButton.UpdateText(GetModeToggleButtonText());
    }

    private void OnLoginSuccess(int? userId, string username)
    {
        _isLoggedIn = true;
        _loggedInUsername = username;
        _loggedInUserId = userId;

        // Rensa input-fält
        _usernameField?.Clear();
        _passwordField?.Clear();

        // Återuppbygg navigator
        RebuildNavigator();
    }

    private void OnLogout()
    {
        _isLoggedIn = false;
        _loggedInUsername = null;
        _loggedInUserId = null;

        // Återuppbygg navigator
        RebuildNavigator();
    }

    private void RebuildNavigator()
    {
        _menuNavigator?.Clear();

        if (_isLoggedIn)
        {
            // Start → Logout → Exit
            _menuNavigator?.AddItem(_menuButtons[0]);
            _menuNavigator?.AddItem(_logoutButton!);
            _menuNavigator?.AddItem(_menuButtons[1]);
        }
        else
        {
            // Username → Password → Action → ModeToggle → Exit
            _menuNavigator?.AddItem(_usernameField!);
            _menuNavigator?.AddItem(_passwordField!);
            _menuNavigator?.AddItem(_actionButton);
            _menuNavigator?.AddItem(_modeToggleButton);
            _menuNavigator?.AddItem(_menuButtons[1]);
        }
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
        return await _apiClient.RegisterUserAsync(username, passwordHash);
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

                // Rensa fält
                _usernameField?.Clear();
                _passwordField?.Clear();

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
            var result = await _apiClient.LoginAsync(username, password);

            if (result.Success)
            {
                ShowMessage($"Välkommen tillbaka {username}!", Color.Green);
                OnLoginSuccess(result.UserId, username);
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
        InitializeLogoutButton();
        SetupMenuNavigation();
    }

    private void InitializeModeToggleButton()
    {
        _modeToggleButton = _uiFactory.CreateModeToggleButton(GetModeToggleButtonText(), OnModeToggleClicked);
    }

    private void InitializeActionButton()
    {
        _actionButton = _uiFactory.CreateActionButton(GetActionButtonText(), OnActionButtonClicked);
    }

    private void InitializeLogoutButton()
    {
        _logoutButton = _uiFactory.CreateLogoutButton(OnLogout);
    }

    private void InitializeMenuComponents()
    {
        if (_menuButtons == null || _menuButtons.Length == 0)
        {
            var startButton = _uiFactory.CreateStartGameButton(() =>
            {
                if (_isLoggedIn)
                {
                    _game.ChangeScene(SceneNames.Battle);
                }
                else
                {
                    ShowMessage("Du måste logga in först!", Color.Red);
                }
            });
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
        }
        RebuildNavigator();
    }

    public override void OnExit()
    {
        if (_menuMusic != null)
        {
            MediaPlayer.Stop();
        }

        _menuNavigator?.Clear();

        if (_modeToggleButton != null)
        {
            _modeToggleButton.OnClick -= OnModeToggleClicked;
        }

        if (_actionButton != null)
        {
            _actionButton.OnClick -= OnActionButtonClicked;
        }

        if (_logoutButton != null)
        {
            _logoutButton.OnClick -= OnLogout;
        }

        base.OnExit();
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = _inputProvider.GetMouseState();
        var keyState = _inputProvider.GetKeyboardState();

        if (_isLoggedIn)
        {
            _menuButtons[0].Update(mouseState); // Start
            _logoutButton?.Update(mouseState);
            _menuButtons[1].Update(mouseState); // Exit

            _menuNavigator?.Update(keyState);
        }
        else
        {
            _usernameField?.Update(gameTime, mouseState);
            _passwordField?.Update(gameTime, mouseState);
            _modeToggleButton?.Update(mouseState);
            _actionButton?.Update(mouseState);
            _menuButtons[1].Update(mouseState); // Exit

            if (_usernameField?.IsFocused == false && _passwordField?.IsFocused == false)
            {
                _menuNavigator?.Update(keyState);
            }
        }

        _feedbackDisplay.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        DrawBackground(spriteBatch);
        DrawTitle(spriteBatch);
        _feedbackDisplay.Draw(spriteBatch, _font, _screenWidth, FeedbackMessageYPosition);

        if (_isLoggedIn)
        {
            DrawLoggedInUI(spriteBatch);
        }
        else
        {
            DrawLoggedOutUI(spriteBatch);
        }
    }

    private void DrawBackground(SpriteBatch spriteBatch)
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
    }

    private void DrawTitle(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _font,
            _title,
            _uiFactory.GetCenteredPosition(_title, TitleYPosition),
            Color.Black
        );
    }

    private void DrawLoggedInUI(SpriteBatch spriteBatch)
    {
        // Rita "Inloggad som: {username}"
        var labelText = $"Inloggad som: {_loggedInUsername}";
        var labelPos = _uiFactory.GetLoggedInLabelPosition(labelText);
        spriteBatch.DrawString(_font, labelText, labelPos, Color.White);

        _menuButtons[0].Draw(spriteBatch); // Start
        _logoutButton?.Draw(spriteBatch);
        _menuButtons[1].Draw(spriteBatch); // Exit
    }

    private void DrawLoggedOutUI(SpriteBatch spriteBatch)
    {
        _usernameField?.Draw(spriteBatch);
        _passwordField?.Draw(spriteBatch);
        _actionButton?.Draw(spriteBatch);
        _modeToggleButton?.Draw(spriteBatch);
        _menuButtons[1].Draw(spriteBatch); // Exit
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _modeToggleButton?.Dispose();
            _actionButton?.Dispose();
            _logoutButton?.Dispose();

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
