using System;
using System.Net.Http;
using System.Threading.Tasks;
using BulletHell.Constants;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.Services;
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
    private const float FeedbackDuration = 3f; // Sekunder

    private readonly SpriteFont _font;
    private readonly Texture2D _whiteTexture;
    private readonly string _title = "Bullet Hell";
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly IMenuInputProvider _inputProvider;
    private readonly ITextInputHandler _textInputHandler;
    private readonly IUserApiClient _userApiClient;
    private readonly IPasswordHasher _passwordHasher;

    private Button[] _menuButtons;
    private InputField[] _menuInputs;
    private InputField? _usernameField;
    private InputField? _passwordField;
    private IMenuNavigator? _menuNavigator;
    private Button _modeToggleButton = null!;
    private Button _actionButton = null!;

    private RegistrationMode _currentMode = RegistrationMode.Login;
    private string _feedbackMessage = string.Empty;
    private Color _feedbackColor = Color.White;
    private float _feedbackTimer = 0f;

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
        _whiteTexture = whiteTexture;
        _inputProvider = inputProvider;
        _textInputHandler = textInputHandler;
        _font = game.Content.Load<SpriteFont>("Font");
        _screenWidth = game.GraphicsDevice.Viewport.Width;
        _screenHeight = game.GraphicsDevice.Viewport.Height;
        _userApiClient = userApiClient;
        _passwordHasher = passwordHasher;
        _menuButtons = [];
        _menuInputs = [];
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
        _whiteTexture = whiteTexture;
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _inputProvider = inputProvider;
        _textInputHandler = textInputHandler;
        _userApiClient = userApiClient;
        _passwordHasher = passwordHasher;
        _menuButtons = [];
        _menuInputs = [];
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
        if (string.IsNullOrWhiteSpace(username))
        {
            return new RegistrationResult
            {
                Success = false,
                Message = "Användarnamn får inte vara tomt",
            };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new RegistrationResult
            {
                Success = false,
                Message = "Lösenord får inte vara tomt",
            };
        }

        if (username.Length < 3)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = "Användarnamn måste vara minst 3 tecken",
            };
        }

        if (password.Length < 6)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = "Lösenord måste vara minst 6 tecken",
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

    private void HandleLogin()
    {
        // TODO: Implementera login senare om behövs
        ShowMessage("Login ej implementerad än", Color.Yellow);
    }

    private void ShowMessage(string message, Color color)
    {
        _feedbackMessage = message;
        _feedbackColor = color;
        _feedbackTimer = FeedbackDuration;
    }

    private Vector2 GetCenteredPosition(string text, float y)
    {
        var size = _font.MeasureString(text);
        return new Vector2(((float)_screenWidth / 2) - (size.X / 2), y);
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
        _modeToggleButton = new Button(
            _font,
            "Mode: Login",
            new Rectangle(
                _game.GraphicsDevice.Viewport.Width / 2 - ButtonWidth / 2,
                100, // Ovanför username field
                ButtonWidth,
                ButtonHeight
            ),
            _whiteTexture
        );

        _modeToggleButton.OnClick += OnModeToggleClicked;

        _actionButton = new Button(
            _font,
            GetActionButtonText(),
            new Rectangle(
                _game.GraphicsDevice.Viewport.Width / 2 - ButtonWidth / 2,
                400,
                ButtonWidth,
                ButtonHeight
            ),
            _whiteTexture
        );

        _actionButton.OnClick += OnActionButtonClicked;

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

    public override void OnExit()
    {
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

        if (_feedbackTimer > 0)
        {
            _feedbackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_feedbackTimer < 0)
            {
                _feedbackMessage = string.Empty;
            }
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

        _modeToggleButton?.Draw(spriteBatch);

        foreach (var inputField in _menuInputs)
        {
            inputField.Draw(spriteBatch);
        }

        _actionButton?.Draw(spriteBatch);

        foreach (var button in _menuButtons)
        {
            button.Draw(spriteBatch);
        }

        if (!string.IsNullOrEmpty(_feedbackMessage))
        {
            var messageSize = _font.MeasureString(_feedbackMessage);
            var messagePosition = new Vector2(_screenWidth / 2 - messageSize.X / 2, 50);

            spriteBatch.DrawString(_font, _feedbackMessage, messagePosition, _feedbackColor);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Note: We don't dispose _whiteTexture here as it's a shared resource
            // owned by Game1 and will be disposed there

            _modeToggleButton?.Dispose();
            _actionButton?.Dispose();

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