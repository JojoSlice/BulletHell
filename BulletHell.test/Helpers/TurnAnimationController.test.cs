using BulletHell.Configurations;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

namespace BulletHell.test.Helpers;

public class TurnAnimationControllerTests
{
    private readonly ISpriteHelper _mockSprite;
    private readonly TurnAnimationController _controller;

    public TurnAnimationControllerTests()
    {
        _mockSprite = Substitute.For<ISpriteHelper>();
        _controller = new TurnAnimationController(_mockSprite);
    }

    [Fact]
    public void Constructor_WithNullSprite_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TurnAnimationController(null!));
    }

    [Fact]
    public void Update_WithNullTurnTextures_DoesNotThrow()
    {
        var direction = new Vector2(1, 0);

        var exception = Record.Exception(() => _controller.Update(direction, null!, null, null));

        Assert.Null(exception);
    }
}