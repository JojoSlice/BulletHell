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
/// Main battle scene where gameplay occurs
/// </summary>
public class BattleScene : Scene
{
    private int _screenWidth;
    private int _screenHeight;
    private Player _player;
    private BulletManager _bulletManager;
    private Texture2D? _playerTexture;
    private Texture2D? _bulletTexture;

    public BattleScene(Game1 game)
        : base(game)
    {
        // Initialize to non-null values
        _player = null!;
        _bulletManager = null!;
    }

    public override void OnEnter()
    {
        // Dispose previous resources if they exist
        _player?.Dispose();
        _bulletManager?.Dispose();

        _screenWidth = _game.GraphicsDevice.Viewport.Width;
        _screenHeight = _game.GraphicsDevice.Viewport.Height;

        // Create player
        Vector2 startPosition = new(_screenWidth / 2, _screenHeight / 2);
        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = new SpriteHelper();
        _player = new Player(startPosition, input, sprite);
        _player.SetScreenBounds(_screenWidth, _screenHeight);

        // Create bullet manager
        _bulletManager = new BulletManager();

        // Load content
        _playerTexture = _game.Content.Load<Texture2D>("player");
        _player.LoadContent(_playerTexture);

        _bulletTexture = _game.Content.Load<Texture2D>("bullet");
        _bulletManager.LoadContent(_bulletTexture);
    }

    public override void Update(GameTime gameTime)
    {
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
            _bulletManager?.Dispose();

            // Note: Textures are managed by ContentManager, don't dispose here
        }

        base.Dispose(disposing);
    }
}
