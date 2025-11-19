using System;
using System.Collections.Generic;

namespace BulletHell.Utilities;

/// <summary>
/// Generic object pool for reusing objects to reduce allocations
/// </summary>
/// <typeparam name="T">Type of object to pool</typeparam>
public class ObjectPool<T> : IDisposable
    where T : class
{
    private readonly Stack<T> _pool;
    private readonly Func<T> _createFunc;
    private readonly Action<T>? _resetAction;
    private readonly int _maxSize;
    private bool _disposed;

    /// <summary>
    /// Gets the current number of objects in the pool
    /// </summary>
    public int Count => _pool.Count;

    /// <summary>
    /// Creates a new object pool
    /// </summary>
    /// <param name="createFunc">Function to create new objects</param>
    /// <param name="resetAction">Optional action to reset object state when returned to pool</param>
    /// <param name="initialSize">Initial number of objects to pre-allocate</param>
    /// <param name="maxSize">Maximum pool size (0 = unlimited)</param>
    public ObjectPool(
        Func<T> createFunc,
        Action<T>? resetAction = null,
        int initialSize = 0,
        int maxSize = 0
    )
    {
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        _resetAction = resetAction;
        _maxSize = maxSize;
        _pool = new Stack<T>(initialSize);

        // Pre-allocate initial objects
        for (int i = 0; i < initialSize; i++)
        {
            _pool.Push(createFunc());
        }
    }

    /// <summary>
    /// Gets an object from the pool, or creates a new one if pool is empty
    /// </summary>
    public T Get()
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop();
        }

        return _createFunc();
    }

    /// <summary>
    /// Returns an object to the pool for reuse
    /// </summary>
    /// <param name="obj">Object to return</param>
    public void Return(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (_maxSize > 0 && _pool.Count >= _maxSize)
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
            return;
        }

        _resetAction?.Invoke(obj);

        _pool.Push(obj);
    }

    /// <summary>
    /// Clears the pool and disposes all objects if they implement IDisposable
    /// </summary>
    public void Clear()
    {
        var items = new List<T>();
        // Collect all remaining objects (ignore Pop errors to be extra defensive)
        while (_pool.Count > 0)
        {
            try
            {
                items.Add(_pool.Pop());
            }
            catch
            {
                break; // Could not pop more items
            }
        }

        // Dispose items that implement IDisposable
        foreach (var obj in items)
        {
            if (obj is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch
                {
                    // Ignore disposal exceptions; continue disposing other objects
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Clear();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
