using System;
using System.Linq;
using System.Net.Http;
using BulletHell.Constants;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.Services;
using BulletHell.UI.Components;
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
    private readonly IMenuInputProvider _inputProvider;
    private readonly IApiClient _apiClient;

    private Button _restartButton = null!;
    private Button _menuButton = null!;
    private Button _exitButton = null!;
    private Song? _gameOverMusic;
    private IMenuNavigator? _menuNavigator;
    private string _highScoreMessage = string.Empty;
    private bool _isProcessingHighScore = false;
    private Leaderboard? _leaderboard;

    public GameOverScene(Game1 game, Texture2D whiteTexture)
        : this(
            game,
            whiteTexture,
            game.Content.Load<SpriteFont>("Font"),
            game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height,
            new MouseKeyboardInputProvider(),
            new ApiClient(new HttpClient { BaseAddress = new Uri("http://localhost:5111") })
        ) { }

    // Constructor for testing - doesn't require Game1.Content or GraphicsDevice
    internal GameOverScene(
        Game1 game,
        Texture2D whiteTexture,
        SpriteFont font,
        int screenWidth,
        int screenHeight,
        IMenuInputProvider inputProvider,
        IApiClient apiClient,
        IMenuNavigator? menuNavigator = null
    )
        : base(game)
    {
        _whiteTexture = whiteTexture;
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _inputProvider = inputProvider;
        _apiClient = apiClient;
        _menuNavigator = menuNavigator;
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
            new Rectangle(
                centerX,
                startY + ButtonHeight + ButtonSpacing,
                ButtonWidth,
                ButtonHeight
            ),
            _whiteTexture
        );
        _menuButton.OnClick += OnMenuClicked;

        _exitButton = new Button(
            _font,
            "Exit Game",
            new Rectangle(
                centerX,
                startY + (ButtonHeight + ButtonSpacing) * 2,
                ButtonWidth,
                ButtonHeight
            ),
            _whiteTexture
        );
        _exitButton.OnClick += OnExitClicked;

        SetupMenuNavigation();

        _leaderboard = new Leaderboard(_font);
        _ = LoadLeaderboardAsync();

        _ = ProcessHighScoreAsync();
    }

    private async System.Threading.Tasks.Task LoadLeaderboardAsync()
    {
        try
        {
            var result = await _apiClient.GetAllHighScoresAsync();
            if (result.Success && result.Data != null)
            {
                _leaderboard?.UpdateHighScores(result.Data);
            }
        }
        catch (HttpRequestException)
        {
            // Ignorera fel vid hämtning av leaderboard
        }
    }

    private async System.Threading.Tasks.Task ProcessHighScoreAsync()
    {
        if (_isProcessingHighScore)
            return;

        _isProcessingHighScore = true;
        _highScoreMessage = string.Empty;

        try
        {
            var finalScore = _game.FinalScore;
            var userId = _game.CurrentUserId;

            if (!userId.HasValue)
            {
                _highScoreMessage = $"Din poäng: {finalScore}";
                return;
            }

            var allHighScoresResult = await _apiClient.GetAllHighScoresAsync();

            if (!allHighScoresResult.Success)
            {
                _highScoreMessage = $"Din poäng: {finalScore}\n(Kunde ej ansluta till server)";
                return;
            }

            var userHighScore = allHighScoresResult.Data?.FirstOrDefault(hs =>
                hs.UserId == userId.Value
            );

            if (userHighScore == null)
            {
                var createResult = await _apiClient.CreateHighScoreAsync(finalScore, userId.Value);

                _highScoreMessage = createResult.Success
                    ? $"Din poäng: {finalScore}\nNYTT HIGHSCORE!"
                    : $"Din poäng: {finalScore}\n(Kunde ej spara highscore)";
            }
            else if (finalScore > userHighScore.Score)
            {
                var updateResult = await _apiClient.UpdateHighScoreAsync(
                    userHighScore.Id,
                    finalScore,
                    userId.Value
                );

                _highScoreMessage = updateResult.Success
                    ? $"Din poäng: {finalScore}\nNYTT HIGHSCORE!\nTidigare rekord: {userHighScore.Score}"
                    : $"Din poäng: {finalScore}\n(Kunde ej uppdatera highscore)";
            }
            else
            {
                _highScoreMessage =
                    $"Din poäng: {finalScore}\nDitt highscore: {userHighScore.Score}";
            }
        }
        catch (Exception ex)
        {
            _highScoreMessage = $"Din poäng: {_game.FinalScore}\n(Fel vid highscore: {ex.Message})";
        }
        finally
        {
            _isProcessingHighScore = false;
        }
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

        _highScoreMessage = string.Empty;
        _isProcessingHighScore = false;

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

        var mouseState = _inputProvider.GetMouseState();
        var keyboardState = _inputProvider.GetKeyboardState();

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
            (float)_screenWidth / 2 - titleSize.X / 2,
            GameOverYPosition
        );

        spriteBatch.DrawString(_font, GameOverText, titlePosition, Color.Red);

        if (!string.IsNullOrEmpty(_highScoreMessage))
        {
            var scoreLines = _highScoreMessage.Split('\n');
            var yOffset = GameOverYPosition + 60;

            foreach (var line in scoreLines)
            {
                var lineSize = _font.MeasureString(line);
                var linePosition = new Vector2((float)_screenWidth / 2 - lineSize.X / 2, yOffset);

                if (line.Contains("NYTT HIGHSCORE"))
                {
                    spriteBatch.DrawString(_font, line, linePosition, Color.Yellow);
                }
                else
                {
                    spriteBatch.DrawString(_font, line, linePosition, Color.White);
                }

                yOffset += 30;
            }
        }
        else if (_isProcessingHighScore)
        {
            var loadingText = "Kontrollerar highscore...";
            var loadingSize = _font.MeasureString(loadingText);
            var loadingPosition = new Vector2(
                (float)_screenWidth / 2 - loadingSize.X / 2,
                GameOverYPosition + 60
            );
            spriteBatch.DrawString(_font, loadingText, loadingPosition, Color.Gray);
        }

        _leaderboard?.Draw(spriteBatch);

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