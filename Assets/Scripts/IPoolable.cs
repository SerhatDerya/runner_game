using UnityEngine;
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
    GameObject GameObject { get; }
}