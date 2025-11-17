using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Scenes;

public class BattleScene(Game1 game) : Scene(game)
{
    private int screenWidth;
    private int screenHeight;
    private Player? _player;

    // konkret typ för prestanda!
    private BulletManager? _bulletManager;
    private Texture2D? playerTexture;
    private Texture2D? bulletTexture;

    public override void OnEnter()
    {
        screenWidth = _game.GraphicsDevice.Viewport.Width;
        screenHeight = _game.GraphicsDevice.Viewport.Height;
        Vector2 startPosition = new(screenWidth / 2, screenHeight / 2);
        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = new SpriteHelper();
        _player = new Player(startPosition, input, sprite);
        _bulletManager = new BulletManager();
        playerTexture = _game.Content.Load<Texture2D>("player");
        _player.LoadContent(playerTexture);
        bulletTexture = _game.Content.Load<Texture2D>("bullet");
        _bulletManager.LoadContent(bulletTexture);

        System.Console.WriteLine("BattleScene loading");
    }

    public override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            _game.Exit();

        _player!.Update(gameTime);
        var shootInfo = _player.TryShoot();
        if (shootInfo.HasValue)
        {
            _bulletManager!.CreateBullet(shootInfo.Value.position, shootInfo.Value.direction);
        }
        _bulletManager!.Update(gameTime, screenWidth, screenHeight);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _player!.Draw(spriteBatch);
        _bulletManager!.Draw(spriteBatch);
    }
}
