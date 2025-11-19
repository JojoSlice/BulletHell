using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;

    private int screenWidth;
    private int screenHeight;

    private SpriteBatch? _spriteBatch;
    private Player? _player;
    private IBulletManager? _bulletManager;
    private IEnemyManager? _enemyManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        screenWidth = _graphics.PreferredBackBufferWidth;
        screenHeight = _graphics.PreferredBackBufferHeight;

        Vector2 startPosition = new(screenWidth / 2, screenHeight / 2);

        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = new SpriteHelper();
        _player = new Player(startPosition, input, sprite);

        _bulletManager = new BulletManager();
        var enemyBulletManager = new EnemyBulletManager();
        _enemyManager = new EnemyManager(enemyBulletManager);
        
        _enemyManager.AddEnemy(new Enemy(
            new Vector2(400, 0), 
            new SpriteHelper()
        ));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D playerTexture = Content.Load<Texture2D>("player");
        _player!.LoadContent(playerTexture);

        Texture2D bulletTexture = Content.Load<Texture2D>("bullet");
        _bulletManager!.LoadContent(bulletTexture);
        
        Texture2D enemyTexture = Content.Load<Texture2D>("enemy");
        _enemyManager!.LoadContent(enemyTexture);

    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        _player!.Update(gameTime);

        var shootInfo = _player.TryShoot();
        if (shootInfo.HasValue)
        {
            _bulletManager!.CreateBullet(shootInfo.Value.position, shootInfo.Value.direction);
        }

        _bulletManager!.Update(gameTime, screenWidth, screenHeight);
        
        _enemyManager!.Update(gameTime, screenWidth, screenHeight);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch!.Begin();
        _player!.Draw(_spriteBatch);
        _bulletManager!.Draw(_spriteBatch);
        _enemyManager!.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
