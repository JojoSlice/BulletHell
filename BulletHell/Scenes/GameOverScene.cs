using System;
using BulletHell.Constants;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BulletHell.Scenes;

public class GameOverScene : Scene
{
    private const string GameOverText = "GAME OVER";
    private const int GameOverYPosition = 150;
    private const int ButtonWidth = 250;
    private const int ButtonHeight = 60;
    private const int ButtonSpacing = 20;

    private readonly SpriteFont _font;
    private readonly Texture2D _whiteTexture;
    private readonly int _screenWidth;
    private readonly int _screenHeight;

    private Button _restartButton = null!;
    private Button _menuButton = null!;
    private Button _exitButton = null!;
    private Song? _gameOverMusic;
    private IMenuNavigator? _menuNavigator;

    public GameOverScene(Game1 game, Texture2D whiteTexture)
        : base(game)
    {
        _whiteTexture = whiteTexture;
        _font = game.Content.Load<SpriteFont>("Font");
        _screenWidth = game.GraphicsDevice.Viewport.Width;
        _screenHeight = game.GraphicsDevice.Viewport.Height;
    }

    public override void OnEnter()
    {
        try
        {
            _gameOverMusic = _game.Content.Load<Song>("Project_137");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(_gameOverMusic);
        }
        catch (Microsoft.Xna.Framework.Audio.NoAudioHardwareException)
        {
            _gameOverMusic = null;
        }

        int startY = 300;
        int centerX = _screenWidth / 2 - ButtonWidth / 2;

        _restartButton = new Button(
            _font,
            "Restart",
            new Rectangle(centerX, startY, ButtonWidth, ButtonHeight),
            _whiteTexture
        );
        _restartButton.OnClick += OnRestartClicked;

        _menuButton = new Button(
            _font,
            "Main Menu",
            new Rectangle(centerX, startY + ButtonHeight + ButtonSpacing, ButtonWidth, ButtonHeight),
            _whiteTexture
        );
        _menuButton.OnClick += OnMenuClicked;

        _exitButton = new Button(
            _font,
            "Exit Game",
            new Rectangle(centerX, startY + (ButtonHeight + ButtonSpacing) * 2, ButtonWidth, ButtonHeight),
            _whiteTexture
        );
        _exitButton.OnClick += OnExitClicked;

        SetupMenuNavigation();
    }

    private void SetupMenuNavigation()
    {
        if (_menuNavigator == null)
        {
            _menuNavigator = new MenuNavigator();
            _menuNavigator.AddItem(_restartButton);
            _menuNavigator.AddItem(_menuButton);
            _menuNavigator.AddItem(_exitButton);
        }
    }

    public override void OnExit()
    {
        if (_gameOverMusic != null)
        {
            MediaPlayer.Stop();
        }

        _menuNavigator?.Clear();

        _restartButton.OnClick -= OnRestartClicked;
        _menuButton.OnClick -= OnMenuClicked;
        _exitButton.OnClick -= OnExitClicked;

        base.OnExit();
    }

    private void OnRestartClicked()
    {
        _game.ChangeScene(SceneNames.Battle);
    }

    private void OnMenuClicked()
    {
        _game.ChangeScene(SceneNames.Menu);
    }

    private void OnExitClicked()
    {
        _game.Exit();
    }

    public override void Update(GameTime gameTime)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var mouseState = Mouse.GetState();
        var keyboardState = Keyboard.GetState();

        _restartButton.Update(mouseState);
        _menuButton.Update(mouseState);
        _exitButton.Update(mouseState);

        _menuNavigator?.Update(keyboardState);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _game.GraphicsDevice.Clear(Color.Black);

        var titleSize = _font.MeasureString(GameOverText);
        var titlePosition = new Vector2(
            _screenWidth / 2 - titleSize.X / 2,
            GameOverYPosition
        );

        spriteBatch.DrawString(_font, GameOverText, titlePosition, Color.Red);

        _restartButton.Draw(spriteBatch);
        _menuButton.Draw(spriteBatch);
        _exitButton.Draw(spriteBatch);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_menuNavigator is IDisposable disposableNavigator)
            {
                disposableNavigator.Dispose();
            }
            _menuNavigator = null;

            _restartButton.Dispose();
            _menuButton.Dispose();
            _exitButton.Dispose();
        }

        base.Dispose(disposing);
    }
}