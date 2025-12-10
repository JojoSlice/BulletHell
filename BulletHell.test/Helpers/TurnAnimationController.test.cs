using BulletHell.Configurations;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using static BulletHell.Helpers.TurnAnimationController;

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

    [Fact]
    public void DetermineNextAction_WithInvalidTextures_ReturnsNoChange()
    {
        var direction = new Vector2(-0.5f, 0);

        var action = _controller.DetermineNextAction(direction, hasValidTextures: false);

        Assert.Equal(SpriteAction.NoChange, action);
    }

    [Theory]
    [InlineData(-0.5f, SpriteAction.StartTurnLeft)]
    [InlineData(0.5f, SpriteAction.StartTurnRight)]
    [InlineData(0.0f, SpriteAction.NoChange)]
    public void DetermineNextAction_FromNoneState_ReturnsCorrectAction(float directionX, SpriteAction expected)
    {
        var direction = new Vector2(directionX, 0);

        var action = _controller.DetermineNextAction(direction, hasValidTextures: true);

        Assert.Equal(expected, action);
    }

    [Fact]
    public void DetermineNextAction_WhenTurningLeftAndNoInput_ReturnsStartExitSequence()
    {
        _controller.CurrentStateForTesting = TurnState.TurningLeft;
        _controller.LastDirectionForTesting = new Vector2(-0.5f, 0);

        var action = _controller.DetermineNextAction(Vector2.Zero, hasValidTextures: true);

        Assert.Equal(SpriteAction.StartExitSequence, action);
    }

    [Fact]
    public void DetermineNextAction_WhenTurningLeftAndSwitchRight_ReturnsStartTurnRight()
    {
        _controller.CurrentStateForTesting = TurnState.TurningLeft;
        _controller.LastDirectionForTesting = new Vector2(-0.5f, 0);

        var rightDirection = new Vector2(0.5f, 0);
        var action = _controller.DetermineNextAction(rightDirection, hasValidTextures: true);

        Assert.Equal(SpriteAction.StartTurnRight, action);
    }

    [Fact]
    public void DetermineNextAction_WhenTurningRightAndSwitchLeft_ReturnsStartTurnLeft()
    {
        _controller.CurrentStateForTesting = TurnState.TurningRight;
        _controller.LastDirectionForTesting = new Vector2(0.5f, 0);

        var leftDirection = new Vector2(-0.5f, 0);
        var action = _controller.DetermineNextAction(leftDirection, hasValidTextures: true);

        Assert.Equal(SpriteAction.StartTurnLeft, action);
    }

    [Fact]
    public void DetermineNextAction_WhenExitingAndComplete_ReturnsReturnToMain()
    {
        _controller.CurrentStateForTesting = TurnState.ExitingTurn;
        _controller.LastDirectionForTesting = Vector2.Zero;
        _mockSprite.IsExitComplete().Returns(true);

        var action = _controller.DetermineNextAction(Vector2.Zero, hasValidTextures: true);

        Assert.Equal(SpriteAction.ReturnToMain, action);
    }

    [Fact]
    public void DetermineNextAction_WhenExitingCompleteAndDetectLeft_ReturnsCompleteExitAndTurnLeft()
    {
        _controller.CurrentStateForTesting = TurnState.ExitingTurn;
        _controller.LastDirectionForTesting = Vector2.Zero;
        _mockSprite.IsExitComplete().Returns(true);

        var action = _controller.DetermineNextAction(new Vector2(-0.5f, 0), hasValidTextures: true);

        Assert.Equal(SpriteAction.CompleteExitAndTurnLeft, action);
    }

    [Fact]
    public void DetermineNextAction_WhenExitingCompleteAndDetectRight_ReturnsCompleteExitAndTurnRight()
    {
        _controller.CurrentStateForTesting = TurnState.ExitingTurn;
        _controller.LastDirectionForTesting = Vector2.Zero;
        _mockSprite.IsExitComplete().Returns(true);

        var action = _controller.DetermineNextAction(new Vector2(0.5f, 0), hasValidTextures: true);

        Assert.Equal(SpriteAction.CompleteExitAndTurnRight, action);
    }

    [Fact]
    public void DetermineNextAction_WhenExitingNotCompleteAndSwitchDirection_ReturnsStartTurnRight()
    {
        _controller.CurrentStateForTesting = TurnState.ExitingTurn;
        _controller.LastDirectionForTesting = new Vector2(-0.5f, 0);
        _mockSprite.IsExitComplete().Returns(false);

        var action = _controller.DetermineNextAction(new Vector2(0.5f, 0), hasValidTextures: true);

        Assert.Equal(SpriteAction.StartTurnRight, action);
    }
}