using System.Collections.Generic;
using BulletHell.Constants;
using BulletHell.Helpers;
using BulletHell.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private Texture2D? _sharedWhiteTexture;
    private Dictionary<string, Scene>? _scenes;
    private Scene? _currentScene;
    private bool _disposed;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        _graphics.IsFullScreen = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = GraphicsAdapter
            .DefaultAdapter
            .CurrentDisplayMode
            .Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter
            .DefaultAdapter
            .CurrentDisplayMode
            .Height;
        _graphics.ApplyChanges();

        _sharedWhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        _sharedWhiteTexture.SetData(new[] { Color.White });

        _scenes = new Dictionary<string, Scene>
        {
            { SceneNames.Menu, new MenuScene(this, _sharedWhiteTexture) },
            { SceneNames.Battle, new BattleScene(this) },
            { SceneNames.GameOver, new GameOverScene(this, _sharedWhiteTexture) },
        };

        _currentScene = _scenes[SceneNames.Menu];
        _currentScene.OnEnter();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        TextureHelper.LoadDefaultFont(Content.Load<SpriteFont>("Font"));
    }

    public void ChangeScene(string sceneName)
    {
        if (_scenes?.ContainsKey(sceneName) == true && _currentScene != null)
        {
            _currentScene.OnExit();
            _currentScene = _scenes[sceneName];
            _currentScene.OnEnter();
        }
    }

    protected override void Update(GameTime gameTime)
    {
        _currentScene?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if (_spriteBatch != null && _currentScene != null)
        {
            var cameraTransform = _currentScene.GetCameraTransform();

            if (cameraTransform.HasValue)
            {
                // Two-pass rendering: World with camera, then HUD without

                // Pass 1: Draw world with camera transform
                _spriteBatch.Begin(transformMatrix: cameraTransform.Value);
                _currentScene.Draw(_spriteBatch);
                _spriteBatch.End();

                // Pass 2: Draw HUD without camera (screen coordinates)
                _spriteBatch.Begin();
                _currentScene.DrawHUD(_spriteBatch);
                _spriteBatch.End();
            }
            else
            {
                // Single-pass rendering (MenuScene or scenes without camera)
                _spriteBatch.Begin();
                _currentScene.Draw(_spriteBatch);
                _spriteBatch.End();
            }
        }

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _sharedWhiteTexture?.Dispose();
                _sharedWhiteTexture = null;

                _spriteBatch?.Dispose();
                _spriteBatch = null;

                // Dispose all scenes
                if (_scenes != null)
                {
                    foreach (var scene in _scenes.Values)
                    {
                        scene.Dispose();
                    }
                    _scenes = null;
                }

                _currentScene = null;
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
