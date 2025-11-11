using System.Collections.Generic;
using BulletHell.Helpers;
using BulletHell.Inputs;
using BulletHell.Interfaces;
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

    private SpriteBatch _spriteBatch;
    private Player _player;

    private List<Bullet> _bullets = [];
    private Texture2D _bulletTexture;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D playerTexture = Content.Load<Texture2D>("player");
        _player.LoadContent(playerTexture);

        _bulletTexture = Content.Load<Texture2D>("bullet");
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        _player.Update(gameTime);

        ISpriteHelper bulletSprite = new SpriteHelper();
        Bullet? newBullet = _player.TryShoot(bulletSprite);
        if (newBullet != null)
        {
            newBullet.LoadContent(_bulletTexture);
            _bullets.Add(newBullet);
        }

        foreach (Bullet bullet in _bullets)
        {
            bullet.Update(gameTime);
        }

        _bullets.RemoveAll(b =>
            !b.IsAlive
            || b.Position.X < 0
            || b.Position.X > screenWidth
            || b.Position.Y < 0
            || b.Position.Y > screenHeight
        );

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _player.Draw(_spriteBatch);

        foreach (Bullet bullet in _bullets)
            bullet.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
