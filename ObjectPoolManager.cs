using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private Dictionary<GameObject, ObjectPool> poolMap = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void CreatePool(GameObject prefab, int size)
    {
        if (!poolMap.ContainsKey(prefab))
        {
            poolMap[prefab] = new ObjectPool(prefab, size, transform);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolMap.ContainsKey(prefab))
            CreatePool(prefab, 1);

        return poolMap[prefab].Get(position, rotation);
    }

    public void Despawn(GameObject obj)
    {
        var identity = obj.GetComponent<PoolIdentity>();
        if (identity != null && poolMap.ContainsKey(identity.Prefab))
        {
            poolMap[identity.Prefab].Return(obj);
        }
        else
        {
            Destroy(obj); // fallback
        }
    }
}
