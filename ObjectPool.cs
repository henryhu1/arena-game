using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject prefab;
    private Transform parent;
    private Queue<GameObject> pool = new();

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        this.prefab.SetActive(false);

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(this.prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Object.Instantiate(prefab, parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        if (obj.TryGetComponent(out IPoolable poolObj)) {
            poolObj.OnSpawned(position);
        }
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj.TryGetComponent(out IPoolable poolObj)) {
            poolObj.OnDespawned();
        }
        obj.transform.parent = parent;
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
