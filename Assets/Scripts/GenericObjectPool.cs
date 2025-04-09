// ObjectPool.cs
using System;
using System.Collections.Generic;

public class GenericObjectPool<T> where T : class
{
    private readonly Queue<T> pool = new();
    private readonly Func<T> factoryMethod;

    public GenericObjectPool(Func<T> factoryMethod, int initialSize)
    {
        this.factoryMethod = factoryMethod;
        for (int i = 0; i < initialSize; i++)
        {
            pool.Enqueue(factoryMethod());
        }
    }

    public T Get()
    {
        return pool.Count > 0 ? pool.Dequeue() : factoryMethod();
    }

    public void Return(T obj)
    {
        pool.Enqueue(obj);
    }
}
