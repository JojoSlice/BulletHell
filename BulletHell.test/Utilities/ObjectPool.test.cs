using BulletHell.Utilities;

namespace BulletHell.test.Utilities;

public class ObjectPoolTests
{
    private class TestObject
    {
        public int Value { get; set; }
        public bool WasReset { get; set; }
    }

    private class DisposableTestObject : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    [Fact]
    public void Constructor_WithNullCreateFunc_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ObjectPool<TestObject>(null!));
    }

    [Fact]
    public void Constructor_WithInitialSize_ShouldPreAllocateObjects()
    {
        // Arrange & Act
        var pool = new ObjectPool<TestObject>(() => new TestObject(), initialSize: 5);

        // Assert
        Assert.Equal(5, pool.Count);
    }

    [Fact]
    public void Constructor_WithZeroInitialSize_ShouldHaveEmptyPool()
    {
        // Arrange & Act
        var pool = new ObjectPool<TestObject>(() => new TestObject(), initialSize: 0);

        // Assert
        Assert.Equal(0, pool.Count);
    }

    [Fact]
    public void Get_WhenPoolHasObjects_ShouldReturnFromPool()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject { Value = 42 }, initialSize: 1);
        var initialCount = pool.Count;

        // Act
        var obj = pool.Get();

        // Assert
        Assert.Equal(42, obj.Value);
        Assert.Equal(initialCount - 1, pool.Count);
    }

    [Fact]
    public void Get_WhenPoolIsEmpty_ShouldCreateNewObject()
    {
        // Arrange
        int createCount = 0;
        var pool = new ObjectPool<TestObject>(() =>
        {
            createCount++;
            return new TestObject { Value = createCount };
        }, initialSize: 0);

        // Act
        var obj1 = pool.Get();
        var obj2 = pool.Get();

        // Assert
        Assert.Equal(1, obj1.Value);
        Assert.Equal(2, obj2.Value);
        Assert.Equal(2, createCount);
    }

    [Fact]
    public void Return_WithValidObject_ShouldAddToPool()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject(), initialSize: 0);
        var obj = new TestObject();

        // Act
        pool.Return(obj);

        // Assert
        Assert.Equal(1, pool.Count);
    }

    [Fact]
    public void Return_WithNullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => pool.Return(null!));
    }

    [Fact]
    public void Return_WithResetAction_ShouldInvokeResetAction()
    {
        // Arrange
        bool resetCalled = false;
        var pool = new ObjectPool<TestObject>(
            () => new TestObject(),
            resetAction: obj => { obj.WasReset = true; resetCalled = true; }
        );
        var obj = new TestObject { WasReset = false };

        // Act
        pool.Return(obj);

        // Assert
        Assert.True(resetCalled);
        Assert.True(obj.WasReset);
    }

    [Fact]
    public void Return_ExceedingMaxSize_ShouldNotAddToPool()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject(), maxSize: 2);
        var obj1 = new TestObject();
        var obj2 = new TestObject();
        var obj3 = new TestObject();

        // Act
        pool.Return(obj1);
        pool.Return(obj2);
        pool.Return(obj3); // Should not be added

        // Assert
        Assert.Equal(2, pool.Count);
    }

    [Fact]
    public void Return_ExceedingMaxSizeWithDisposable_ShouldDisposeObject()
    {
        // Arrange
        var pool = new ObjectPool<DisposableTestObject>(() => new DisposableTestObject(), maxSize: 1);
        var obj1 = new DisposableTestObject();
        var obj2 = new DisposableTestObject();

        // Act
        pool.Return(obj1);
        pool.Return(obj2); // Should be disposed

        // Assert
        Assert.Equal(1, pool.Count);
        Assert.False(obj1.IsDisposed);
        Assert.True(obj2.IsDisposed);
    }

    [Fact]
    public void GetAndReturn_ShouldReuseObjects()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject { Value = 99 });
        var obj = pool.Get();

        // Act
        pool.Return(obj);
        var reusedObj = pool.Get();

        // Assert
        Assert.Same(obj, reusedObj);
    }

    [Fact]
    public void Clear_ShouldRemoveAllObjects()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject(), initialSize: 5);
        Assert.Equal(5, pool.Count);

        // Act
        pool.Clear();

        // Assert
        Assert.Equal(0, pool.Count);
    }

    [Fact]
    public void Clear_WithDisposableObjects_ShouldDisposeAll()
    {
        // Arrange
        var obj1 = new DisposableTestObject();
        var obj2 = new DisposableTestObject();
        var obj3 = new DisposableTestObject();

        var pool = new ObjectPool<DisposableTestObject>(() => new DisposableTestObject());
        pool.Return(obj1);
        pool.Return(obj2);
        pool.Return(obj3);

        // Act
        pool.Clear();

        // Assert
        Assert.Equal(0, pool.Count);
        Assert.True(obj1.IsDisposed);
        Assert.True(obj2.IsDisposed);
        Assert.True(obj3.IsDisposed);
    }

    [Fact]
    public void Count_ShouldTrackPoolSize()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject(), initialSize: 3);

        // Act & Assert
        Assert.Equal(3, pool.Count);

        var obj = pool.Get();
        Assert.Equal(2, pool.Count);

        pool.Return(obj);
        Assert.Equal(3, pool.Count);

        pool.Clear();
        Assert.Equal(0, pool.Count);
    }

    [Fact]
    public void MaxSize_Zero_ShouldAllowUnlimitedObjects()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject(), maxSize: 0);

        // Act - return many objects
        for (int i = 0; i < 100; i++)
        {
            pool.Return(new TestObject());
        }

        // Assert
        Assert.Equal(100, pool.Count);
    }

    [Fact]
    public void Pool_WithMultipleGetAndReturn_ShouldMaintainCorrectCount()
    {
        // Arrange
        var pool = new ObjectPool<TestObject>(() => new TestObject(), initialSize: 2);

        // Act
        var obj1 = pool.Get();
        var obj2 = pool.Get();
        Assert.Equal(0, pool.Count);

        pool.Return(obj1);
        Assert.Equal(1, pool.Count);

        var obj3 = pool.Get();
        Assert.Equal(0, pool.Count);
        Assert.Same(obj1, obj3);

        pool.Return(obj2);
        pool.Return(obj3);
        Assert.Equal(2, pool.Count);

        // Assert
        var obj4 = pool.Get();
        var obj5 = pool.Get();
        Assert.NotNull(obj4);
        Assert.NotNull(obj5);
        Assert.Equal(0, pool.Count);
    }
}
