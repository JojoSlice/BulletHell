using System.Collections.Generic;
using BulletHell.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;

    private Dictionary<string, Scene> scenes = null!;
    private Scene currentScene = null!;

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

        // Skapa alla scener
        scenes = new Dictionary<string, Scene>
        {
            { "Menu", new MenuScene(this) },
            { "Battle", new BattleScene(this) },
        };

        // Starta med menyn
        currentScene = scenes["Menu"];
        currentScene.OnEnter();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public void ChangeScene(string sceneName)
    {
        if (scenes.ContainsKey(sceneName))
        {
            currentScene.OnExit();
            currentScene = scenes[sceneName];
            currentScene.OnEnter();
        }
    }

    protected override void Update(GameTime gameTime)
    {
        currentScene.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch!.Begin();
        currentScene.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
