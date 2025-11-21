using BulletHell.Interfaces;
using BulletHell.Managers;
using Microsoft.Xna.Framework.Input;
using NSubstitute;

namespace BulletHell.test.Managers;

public class MenuNavigatorTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var navigator = new MenuNavigator();

        // Assert
        Assert.NotNull(navigator);
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void AddItem_FirstItem_ShouldSelectIt()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var mockItem = Substitute.For<INavigable>();

        // Act
        navigator.AddItem(mockItem);

        // Assert
        mockItem.Received(1).SetSelected(true);
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void AddItem_SecondItem_ShouldNotSelectIt()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var firstItem = Substitute.For<INavigable>();
        var secondItem = Substitute.For<INavigable>();

        navigator.AddItem(firstItem);

        // Act
        navigator.AddItem(secondItem);

        // Assert
        secondItem.DidNotReceive().SetSelected(true);
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void AddItem_MultipleItems_ShouldOnlySelectFirst()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var items = new[]
        {
            Substitute.For<INavigable>(),
            Substitute.For<INavigable>(),
            Substitute.For<INavigable>()
        };

        // Act
        foreach (var item in items)
        {
            navigator.AddItem(item);
        }

        // Assert
        items[0].Received(1).SetSelected(true);
        items[1].DidNotReceive().SetSelected(Arg.Any<bool>());
        items[2].DidNotReceive().SetSelected(Arg.Any<bool>());
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void NavigateDown_WithMultipleItems_ShouldMoveToNextItem()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Act
        navigator.NavigateDown();

        // Assert
        item1.Received(1).SetSelected(false);
        item2.Received(1).SetSelected(true);
        Assert.Equal(1, navigator.SelectedIndex);
    }

    [Fact]
    public void NavigateDown_AtLastItem_ShouldWrapToFirst()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();
        var item3 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);
        navigator.AddItem(item3);

        // Move to last item
        navigator.NavigateDown(); // Index 1
        navigator.NavigateDown(); // Index 2

        // Act - should wrap to first
        navigator.NavigateDown();

        // Assert
        item3.Received(1).SetSelected(false);
        item1.Received(2).SetSelected(true); // Once on add, once on wrap
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void NavigateDown_WithEmptyList_ShouldNotThrow()
    {
        // Arrange
        var navigator = new MenuNavigator();

        // Act & Assert
        var exception = Record.Exception(() => navigator.NavigateDown());
        Assert.Null(exception);
    }

    [Fact]
    public void NavigateUp_WithMultipleItems_ShouldMoveToPreviousItem()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Move to second item first
        navigator.NavigateDown();

        // Act
        navigator.NavigateUp();

        // Assert
        item2.Received(1).SetSelected(false);
        item1.Received(2).SetSelected(true); // Once on add, once on navigate up
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void NavigateUp_AtFirstItem_ShouldWrapToLast()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();
        var item3 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);
        navigator.AddItem(item3);

        // Act - navigate up from first item
        navigator.NavigateUp();

        // Assert
        item1.Received(1).SetSelected(false);
        item3.Received(1).SetSelected(true);
        Assert.Equal(2, navigator.SelectedIndex);
    }

    [Fact]
    public void NavigateUp_WithEmptyList_ShouldNotThrow()
    {
        // Arrange
        var navigator = new MenuNavigator();

        // Act & Assert
        var exception = Record.Exception(() => navigator.NavigateUp());
        Assert.Null(exception);
    }

    [Fact]
    public void NavigateUp_WithSingleItem_ShouldStayOnSameItem()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item = Substitute.For<INavigable>();
        navigator.AddItem(item);

        // Act
        navigator.NavigateUp();

        // Assert
        item.Received(1).SetSelected(false);
        item.Received(2).SetSelected(true); // Once on add, once on navigate
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void ActivateSelected_WithItems_ShouldCallActivateOnSelectedItem()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Act
        navigator.ActivateSelected();

        // Assert
        item1.Received(1).Activate();
        item2.DidNotReceive().Activate();
    }

    [Fact]
    public void ActivateSelected_AfterNavigation_ShouldActivateCorrectItem()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);
        navigator.NavigateDown(); // Move to item2

        // Act
        navigator.ActivateSelected();

        // Assert
        item1.DidNotReceive().Activate();
        item2.Received(1).Activate();
    }

    [Fact]
    public void ActivateSelected_WithEmptyList_ShouldNotThrow()
    {
        // Arrange
        var navigator = new MenuNavigator();

        // Act & Assert
        var exception = Record.Exception(() => navigator.ActivateSelected());
        Assert.Null(exception);
    }

    [Fact]
    public void Update_WithWKey_ShouldNavigateUp()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Move to second item
        var currentState = new KeyboardState(Keys.S);
        navigator.Update(currentState);
        navigator.Update(new KeyboardState()); // Release key

        // Act - press W
        currentState = new KeyboardState(Keys.W);
        navigator.Update(currentState);

        // Assert
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void Update_WithUpArrowKey_ShouldNavigateUp()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Move to second item first
        navigator.NavigateDown();

        // Act
        var currentState = new KeyboardState(Keys.Up);
        navigator.Update(currentState);

        // Assert
        Assert.Equal(0, navigator.SelectedIndex);
    }

    [Fact]
    public void Update_WithSKey_ShouldNavigateDown()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Act
        var currentState = new KeyboardState(Keys.S);
        navigator.Update(currentState);

        // Assert
        Assert.Equal(1, navigator.SelectedIndex);
    }

    [Fact]
    public void Update_WithDownArrowKey_ShouldNavigateDown()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);

        // Act
        var currentState = new KeyboardState(Keys.Down);
        navigator.Update(currentState);

        // Assert
        Assert.Equal(1, navigator.SelectedIndex);
    }

    [Fact]
    public void Update_WithEnterKey_ShouldActivateSelected()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item = Substitute.For<INavigable>();
        navigator.AddItem(item);

        // Act
        var currentState = new KeyboardState(Keys.Enter);
        navigator.Update(currentState);

        // Assert
        item.Received(1).Activate();
    }

    [Fact]
    public void Update_WithSpaceKey_ShouldActivateSelected()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item = Substitute.For<INavigable>();
        navigator.AddItem(item);

        // Act
        var currentState = new KeyboardState(Keys.Space);
        navigator.Update(currentState);

        // Assert
        item.Received(1).Activate();
    }

    [Fact]
    public void Update_WithHeldKey_ShouldNotRepeatAction()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();
        var item3 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);
        navigator.AddItem(item3);

        // Act - press S once, then keep holding
        var keyState = new KeyboardState(Keys.S);
        navigator.Update(keyState); // Should navigate down
        navigator.Update(keyState); // Should NOT navigate (key still held)
        navigator.Update(keyState); // Should NOT navigate (key still held)

        // Assert - should only have moved once (to index 1)
        Assert.Equal(1, navigator.SelectedIndex);
    }

    [Fact]
    public void Update_WithNoKeys_ShouldDoNothing()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item = Substitute.For<INavigable>();
        navigator.AddItem(item);

        // Act
        var currentState = new KeyboardState();
        navigator.Update(currentState);

        // Assert
        Assert.Equal(0, navigator.SelectedIndex);
        item.DidNotReceive().Activate();
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var navigator = new MenuNavigator();
        var item1 = Substitute.For<INavigable>();
        var item2 = Substitute.For<INavigable>();

        navigator.AddItem(item1);
        navigator.AddItem(item2);
        navigator.NavigateDown(); // Move to index 1

        // Act
        navigator.Clear();

        // Assert
        Assert.Equal(0, navigator.SelectedIndex);
        var exception = Record.Exception(() => navigator.ActivateSelected());
        Assert.Null(exception); // Should not throw (no items to activate)
    }

    [Fact]
    public void Clear_OnEmptyNavigator_ShouldNotThrow()
    {
        // Arrange
        var navigator = new MenuNavigator();

        // Act & Assert
        var exception = Record.Exception(() => navigator.Clear());
        Assert.Null(exception);
    }
}
