using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Scenes;

public class MenuScene : Scene
{
    private readonly SpriteFont font;
    private readonly string title = "HUVUDMENY";
    private readonly string startText = "Tryck ENTER för att starta";

    public MenuScene(Game1 game)
        : base(game)
    {
        font = game.Content.Load<SpriteFont>("Font"); // Ladda din font
    }

    public override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();

        if (keyState.IsKeyDown(Keys.Enter))
        {
            _game.ChangeScene("Battle");
        }

        if (keyState.IsKeyDown(Keys.Escape))
        {
            _game.Exit();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, title, new Vector2(300, 200), Color.White);
        spriteBatch.DrawString(font, startText, new Vector2(250, 300), Color.Yellow);
        spriteBatch.DrawString(font, "ESC för att avsluta", new Vector2(250, 350), Color.Gray);
    }

    public override void OnEnter()
    {
        // Saker som ska hända när man kommer till menyn
        System.Console.WriteLine("Meny laddad");
    }
}
