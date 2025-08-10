using UnityEngine;

public interface IPoolable
{
    void OnSpawned(Vector3 pos);
    void OnDespawned();
}
