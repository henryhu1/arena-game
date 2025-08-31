using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticle : MonoBehaviour, IPoolable
{
    private ParticleSystem ps;

    // TODO: consider having the particle scriptable object in this class
    private GameObject prefabRef;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void OnSpawned(Vector3 spawnPos)
    {
        ps.Clear(true);
        ps.Play(true);
    }

    public void OnDespawned()
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnParticleSystemStopped()
    {
        if (prefabRef != null)
        {
            ObjectPoolManager.Instance.Despawn(gameObject, prefabRef);
        }
    }

    public void SetPrefabReference(GameObject prefab)
    {
        prefabRef = prefab;
    }
}
