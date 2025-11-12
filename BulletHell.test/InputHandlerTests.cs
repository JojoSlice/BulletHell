using BulletHell.Interfaces;
using Microsoft.Xna.Framework;

namespace BulletHell.test;

public class FakeInputReader : IInputProvider
{
    private readonly string _input;

    public FakeInputReader(string input) => _input = input;

    public Vector2 GetDirection()
    {
        return _input.ToUpper() switch
        {
            "UP" => new Vector2(0, -1),
            "DOWN" => new Vector2(0, 1),
            "LEFT" => new Vector2(-1, 0),
            "RIGHT" => new Vector2(1, 0),
            _ => Vector2.Zero
        };
    }

    public bool IsShootPressed()
    {
        throw new NotImplementedException();
    }
}

public class InputHandlerTests
{
    public class InputProviderTests
    {
        [Theory]
        [InlineData("UP", 0, -1)]
        [InlineData("DOWN", 0, 1)]
        [InlineData("LEFT", -1, 0)]
        [InlineData("RIGHT", 1, 0)]
        [InlineData("INVALID", 0, 0)]
        public void GetDirection_Returns_Correct_Vector(string input, float expectedX, float expectedY)
        {
            // Arrange
            var fakeInput = new FakeInputReader(input);

            // Act
            var direction = fakeInput.GetDirection();

            // Assert
            Assert.Equal(new Vector2(expectedX, expectedY), direction);
        }
    }
}