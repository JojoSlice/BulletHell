using BulletHell.Constants;
using BulletHell.Factories;
using BulletHell.Graphics;
using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BulletHell.Scenes;

/// <summary>
/// Main battle scene where gameplay occurs.
/// </summary>
public class BattleScene : Scene
{
    private int _screenWidth;
    private int _screenHeight;
    private Player? _player;
    private BulletManager<Player>? _bulletManager;
    private EnemyManager? _enemyManager;
    private BulletManager<Enemy>? _enemyBulletManager;
    private CollisionManager? _collisionManager;
    private Texture2D? _playerTexture;
    private Texture2D? _bulletTexture;
    private Texture2D? _enemyTexture;
    private Texture2D? _enemyBulletTexture;
    private HUD? _hud;
    private Camera? _camera;
    private Texture2D? _lifeTexture;
    private Texture2D? _backgroundTexture;
    private Texture2D? _backgroundTexture2;
    private Texture2D? _backgroundTexture3;
    private Texture2D? _backgroundTexture4;
    private DashManager? _dashManager;
    private Texture2D? _dashTexture;
    private float _dashSpawnTimer = 0f;
    private RymdDashManager? _rymdDashManager;
    private Texture2D? _rymdDashTexture1;
    private Texture2D? _rymdDashTexture2;
    private Texture2D? _rymdDashTexture3;
    private float _rymdDashSpawnTimer = 0f;
    private ExplosionManager? _explosionManager;
    private Texture2D? _explosionTexture;

    public BattleScene(Game1 game)
        : base(game)
    {
    }

    public override void OnEnter()
    {
        _player?.Dispose();
        _bulletManager?.Dispose();
        _enemyBulletManager?.Dispose();

        _screenWidth = _game.GraphicsDevice.Viewport.Width;
        _screenHeight = _game.GraphicsDevice.Viewport.Height;

        // Create factory for sprite helpers
        ISpriteHelperFactory spriteHelperFactory = new SpriteHelperFactory();

        Vector2 startPosition = new((float)_screenWidth / 2, (float)_screenHeight / 2);
        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = spriteHelperFactory.Create();
        var turnAnimationController = new TurnAnimationController(sprite);
        _player = new Player(startPosition, input, sprite, turnAnimationController);
        _player.SetScreenBounds(_screenWidth, _screenHeight);

        _bulletManager = new BulletManager<Player>(spriteHelperFactory);
        _enemyBulletManager = new BulletManager<Enemy>(spriteHelperFactory);
        _enemyManager = new EnemyManager(_enemyBulletManager, spriteHelperFactory);
        _explosionManager = new ExplosionManager();
        _dashManager = new DashManager();
        _rymdDashManager = new RymdDashManager();

        _hud = new HUD();
        _hud.MaxHP = 100;
        _hud.HP = _player.Health;

        _lifeTexture = _game.Content.Load<Texture2D>("player_Life");
        _hud.LifeTexture = _lifeTexture;

        _enemyManager.AddEnemy(new Enemy(new Vector2(400, 0), spriteHelperFactory.Create()));

        _backgroundTexture = _game.Content.Load<Texture2D>("rymdbg");
        _backgroundTexture2 = _game.Content.Load<Texture2D>("rymdbg2");
        _backgroundTexture3 = _game.Content.Load<Texture2D>("rymdbg3");
        _backgroundTexture4 = _game.Content.Load<Texture2D>("rymdbg4");

        // Add initial enemy
        _enemyManager.AddEnemy(new Enemy(new Vector2(400, 0), spriteHelperFactory.Create()));

        _playerTexture = _game.Content.Load<Texture2D>("rymdskepp");
        var turnLeftTexture = _game.Content.Load<Texture2D>("rymdskeppturnleft");
        var turnRightTexture = _game.Content.Load<Texture2D>("rymdskeppturnright");
        _player.LoadContent(_playerTexture, turnLeftTexture, turnRightTexture);

        _bulletTexture = _game.Content.Load<Texture2D>("bullet");
        _bulletManager.LoadContent(_bulletTexture);

        _enemyTexture = _game.Content.Load<Texture2D>("enemy");
        _enemyManager.LoadContent(_enemyTexture);

        _enemyBulletTexture = _game.Content.Load<Texture2D>("enemy_bullet");
        _enemyBulletManager.LoadContent(_enemyBulletTexture);

        _explosionTexture = _game.Content.Load<Texture2D>("enemy_explosion");
        _explosionManager.LoadContent(_explosionTexture);

        _collisionManager = new CollisionManager(
            _player,
            _bulletManager,
            _enemyManager,
            _enemyBulletManager,
            _explosionManager,
            spriteHelperFactory
        );

        _dashTexture = _game.Content.Load<Texture2D>("dash");
        _dashManager.LoadContent(_dashTexture);

        _rymdDashTexture1 = _game.Content.Load<Texture2D>("rymddash1");
        _rymdDashTexture2 = _game.Content.Load<Texture2D>("rymddash2");
        _rymdDashTexture3 = _game.Content.Load<Texture2D>("rymddash3");
        _rymdDashManager.LoadContent(_rymdDashTexture1, _rymdDashTexture2, _rymdDashTexture3);
        _camera = new Camera();
        _camera.SetWorldBounds((float)_screenWidth * 2, (float)_screenHeight * 2);
    }

    public override void Update(GameTime gameTime)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
        {
            _game.Exit();
            return;
        }

        if (
            _player == null
            || _bulletManager == null
            || _enemyManager == null
            || _enemyBulletManager == null
        )
            return;

        // Spawn dash effects periodically
        _dashSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_dashSpawnTimer >= 0.2f)
        {
            _dashManager?.SpawnDash(_screenWidth);
            _dashSpawnTimer = 0f;
        }

        // Spawn rymddash effects less frequently
        _rymdDashSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_rymdDashSpawnTimer >= 0.7f)
        {
            _rymdDashManager?.SpawnRymdDash(_screenWidth);
            _rymdDashSpawnTimer = 0f;
        }

        _dashManager?.Update(gameTime, _screenWidth, _screenHeight);
        _rymdDashManager?.Update(gameTime, _screenWidth, _screenHeight);
        _collisionManager?.CheckCollisions();
        _player.Update(gameTime);
        _camera?.Follow(_player.Position, _game.GraphicsDevice.Viewport, 0.1f);

        var shootInfo = _player.TryShoot();
        if (shootInfo.HasValue)
        {
            _bulletManager.CreateBullet(shootInfo.Value.position, shootInfo.Value.direction);
        }

        _bulletManager.Update(gameTime, _screenWidth, _screenHeight);
        _enemyManager.Update(gameTime, _screenWidth, _screenHeight);
        _enemyBulletManager.Update(gameTime, _screenWidth, _screenHeight);
        _explosionManager?.Update(gameTime);

        if (_hud != null)
        {
            _hud.HP = _player.Health;
            _hud.Lives = _player.Lives;
            _hud.UpdateScore(_player.Score);
        }

        if (!_player.IsAlive)
        {
            _game.FinalScore = _player.Score;
            _game.ChangeScene(SceneNames.GameOver);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (
            _player == null
            || _bulletManager == null
            || _enemyManager == null
            || _enemyBulletManager == null
        )
            return;

        // Draw multi-layer parallax backgrounds (back to front)
        if (_camera != null)
        {
            // Helper method to draw a parallax layer
            void DrawParallaxLayer(Texture2D? texture, float parallaxFactor, float scale = 1.5f)
            {
                if (texture == null)
                {
                    return;
                }

                Vector2 bgPosition = _camera.Position * parallaxFactor;
                Rectangle destRect = new Rectangle(
                    (int)bgPosition.X,
                    (int)bgPosition.Y,
                    (int)(_screenWidth * scale),
                    (int)(_screenHeight * scale)
                );
                spriteBatch.Draw(texture, destRect, Color.White);
            }

            // Draw layers from back to front with very slow speeds
            DrawParallaxLayer(_backgroundTexture, 0.1f); // Furthest, slowest
            DrawParallaxLayer(_backgroundTexture2, 0.15f); // Mid-back
            DrawParallaxLayer(_backgroundTexture3, 0.2f); // Mid-front
            DrawParallaxLayer(_backgroundTexture4, 0.25f); // Closest, fastest
        }

        _rymdDashManager?.Draw(spriteBatch, _screenWidth, _screenHeight);
        _dashManager?.Draw(spriteBatch);
        _player.Draw(spriteBatch);
        _bulletManager.Draw(spriteBatch);
        _enemyManager.Draw(spriteBatch);
        _enemyBulletManager.Draw(spriteBatch);
        _explosionManager?.Draw(spriteBatch);
    }

    public override Matrix? GetCameraTransform()
    {
        return _camera?.Transform;
    }

    public override void DrawBackground(SpriteBatch spriteBatch)
    {
        // Background is now drawn with parallax in Draw() method
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        if (_hud != null)
            _hud.Draw(spriteBatch);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _player?.Dispose();
            _player = null;

            _bulletManager?.Dispose();
            _bulletManager = null;

            _enemyBulletManager?.Dispose();
            _enemyBulletManager = null;

            _enemyManager = null;

            _dashManager?.Dispose();
            _dashManager = null;

            _rymdDashManager?.Dispose();
            _rymdDashManager = null;

            // Note: Textures are managed by ContentManager, don't dispose here
            _playerTexture = null;
            _bulletTexture = null;
            _enemyTexture = null;
            _enemyBulletTexture = null;
            _backgroundTexture = null;
            _backgroundTexture2 = null;
            _backgroundTexture3 = null;
            _backgroundTexture4 = null;
            _dashTexture = null;
            _rymdDashTexture1 = null;
            _rymdDashTexture2 = null;
            _rymdDashTexture3 = null;
            _camera = null;
        }

        base.Dispose(disposing);
    }
}