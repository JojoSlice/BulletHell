using BulletHell.Graphics;
using System;
using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Scenes;

/// <summary>
/// Main battle scene where gameplay occurs.
/// </summary>
public class BattleScene : Scene
{
    private int _screenWidth;
    private int _screenHeight;
    private Player? _player;
    private BulletManager? _bulletManager;
    private IEnemyManager? _enemyManager;
    private EnemyBulletManager? _enemyBulletManager;
    private Texture2D? _playerTexture;
    private Texture2D? _bulletTexture;
    private Texture2D? _enemyTexture;
    private Texture2D? _enemyBulletTexture;
    private HUD? _hud;

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

        Vector2 startPosition = new((float)_screenWidth / 2, (float)_screenHeight / 2);
        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = new SpriteHelper();
        _player = new Player(startPosition, input, sprite);
        _player.SetScreenBounds(_screenWidth, _screenHeight);

        _bulletManager = new BulletManager();
        _enemyBulletManager = new EnemyBulletManager();
        _enemyManager = new EnemyManager(_enemyBulletManager);

        _hud = new HUD();
        _hud.MaxHP = 100; // eller vad du vill
        _hud.HP = _player.Health; // Koppla spelarens HP

        // Add initial enemy
        _enemyManager.AddEnemy(new Enemy(
            new Vector2(400, 0),
            new SpriteHelper()
        ));

        _playerTexture = _game.Content.Load<Texture2D>("player");
        _player.LoadContent(_playerTexture);

        _bulletTexture = _game.Content.Load<Texture2D>("bullet");
        _bulletManager.LoadContent(_bulletTexture);

        _enemyTexture = _game.Content.Load<Texture2D>("enemy");
        _enemyManager.LoadContent(_enemyTexture);

        _enemyBulletTexture = _game.Content.Load<Texture2D>("enemy_bullet");
        _enemyBulletManager.LoadContent(_enemyBulletTexture);
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

        if (_player == null || _bulletManager == null || _enemyManager == null || _enemyBulletManager == null)
            return;

        _player.Update(gameTime);

        var shootInfo = _player.TryShoot();
        if (shootInfo.HasValue)
        {
            _bulletManager.CreateBullet(shootInfo.Value.position, shootInfo.Value.direction);
        }

        _bulletManager.Update(gameTime, _screenWidth, _screenHeight);
        _enemyManager.Update(gameTime, _screenWidth, _screenHeight);
        _enemyBulletManager.Update(gameTime, _screenWidth, _screenHeight);

        if (_hud != null)
        {
            _hud.HP = _player.Health;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_player == null || _bulletManager == null || _enemyManager == null || _enemyBulletManager == null)
            return;

        _player.Draw(spriteBatch);
        _bulletManager.Draw(spriteBatch);
        _enemyManager.Draw(spriteBatch);
        _enemyBulletManager.Draw(spriteBatch);

        if (_hud != null)
        {
            _hud.Draw(spriteBatch);
        }
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

            // Note: Textures are managed by ContentManager, don't dispose here
            _playerTexture = null;
            _bulletTexture = null;
            _enemyTexture = null;
            _enemyBulletTexture = null;
        }

        base.Dispose(disposing);
    }
}