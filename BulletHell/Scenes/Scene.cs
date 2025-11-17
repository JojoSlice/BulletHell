using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Scenes;

public abstract class Scene(Game1 game)
{
    protected Game1 _game = game;

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void OnEnter() { }

    public virtual void OnExit() { }
}
