using System;
using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
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
    private Texture2D? _playerTexture;
    private Texture2D? _bulletTexture;

    public BattleScene(Game1 game)
        : base(game)
    {
    }

    public override void OnEnter()
    {
        _player?.Dispose();
        _bulletManager?.Dispose();

        _screenWidth = _game.GraphicsDevice.Viewport.Width;
        _screenHeight = _game.GraphicsDevice.Viewport.Height;

        Vector2 startPosition = new((float)_screenWidth / 2, (float)_screenHeight / 2);
        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = new SpriteHelper();
        _player = new Player(startPosition, input, sprite);
        _player.SetScreenBounds(_screenWidth, _screenHeight);

        _bulletManager = new BulletManager();

        _playerTexture = _game.Content.Load<Texture2D>("player");
        _player.LoadContent(_playerTexture);

        _bulletTexture = _game.Content.Load<Texture2D>("bullet");
        _bulletManager.LoadContent(_bulletTexture);
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

        if (_player == null || _bulletManager == null)
            return;

        _player.Update(gameTime);

        var shootInfo = _player.TryShoot();
        if (shootInfo.HasValue)
        {
            _bulletManager.CreateBullet(shootInfo.Value.position, shootInfo.Value.direction);
        }

        _bulletManager.Update(gameTime, _screenWidth, _screenHeight);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_player == null || _bulletManager == null)
            return;

        _player.Draw(spriteBatch);
        _bulletManager.Draw(spriteBatch);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _player?.Dispose();
            _player = null;

            _bulletManager?.Dispose();
            _bulletManager = null;

            // Note: Textures are managed by ContentManager, don't dispose here
            _playerTexture = null;
            _bulletTexture = null;
        }

        base.Dispose(disposing);
    }
}
