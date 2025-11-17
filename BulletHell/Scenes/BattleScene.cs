using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Scenes;

public class BattleScene : Scene
{
    private int screenWidth;
    private int screenHeight;

    private Player? _player;
    private IBulletManager? _bulletManager;

    private Texture2D? playerTexture;
    private Texture2D? bulletTexture;

    public BattleScene(Game1 game)
        : base(game)
    {
        screenWidth = game.GraphicsDevice.Viewport.Width;
        screenHeight = game.GraphicsDevice.Viewport.Height;

        Vector2 startPosition = new(screenWidth / 2, screenHeight / 2);

        IInputProvider input = new KeyboardInputProvider();
        ISpriteHelper sprite = new SpriteHelper();
        _player = new Player(startPosition, input, sprite);

        _bulletManager = new BulletManager();

        playerTexture = game.Content.Load<Texture2D>("player");
        _player.LoadContent(playerTexture);

        bulletTexture = game.Content.Load<Texture2D>("bullet");
        _bulletManager.LoadContent(bulletTexture);
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

    public override void OnEnter()
    {
        System.Console.WriteLine("Battle scene laddad");
    }
}
