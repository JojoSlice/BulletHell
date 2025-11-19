using System.Collections.Generic;
using BulletHell.Constants;
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

        // Fullscreen
        _graphics.IsFullScreen = true;
    }

    protected override void Initialize()
    {
        // Sätt fönsterstorlek till skärmstorlek
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.ApplyChanges();

        // Create shared white texture for UI elements
        _sharedWhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        _sharedWhiteTexture.SetData(new[] { Color.White });

        // Skapa alla scener
        _scenes = new Dictionary<string, Scene>
        {
            { SceneNames.Menu, new MenuScene(this, _sharedWhiteTexture) },
            { SceneNames.Battle, new BattleScene(this) },
        };

        // Starta med menyn
        _currentScene = _scenes[SceneNames.Menu];
        _currentScene.OnEnter();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
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
            _spriteBatch.Begin();
            _currentScene.Draw(_spriteBatch);
            _spriteBatch.End();
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
