using UnityEngine;

[CreateAssetMenu(fileName = "ParticleEffectSO", menuName = "Particles/Effects")]
public class ParticleEffectSO : ScriptableObject
{
    public GameObject particlePrefab;
    public int initialPoolSize = 5;

    public bool isPoolInitialized = false;

    [Header("Event Hookup")]
    public Vector3EventChannelSO triggerEvent;

    public void InitializePoolIfNeeded()
    {
        if (particlePrefab != null && !isPoolInitialized)
        {
            Debug.Log("initialized pool");
            ObjectPoolManager.Instance.CreatePool(particlePrefab, initialPoolSize);
            isPoolInitialized = true;
        }

        Subscribe();
    }

    public void Dispose()
    {
        Unsubscribe();
    }

    public void Play(Vector3 position, Quaternion? rotation = null)
    {
        Debug.Log("playing particle");
        if (particlePrefab != null)
        {
            // InitializePoolIfNeeded(); DANGER: doubles particles played

            var rot = rotation ?? Quaternion.identity;

            ObjectPoolManager.Instance.Spawn(particlePrefab, position, rot);
        }
    }

    public void Play(Vector3 pos) => Play(pos, Quaternion.identity);

    private void Subscribe()
    {
        if (triggerEvent != null)
            triggerEvent.OnPositionEventRaised += Play;

        // if (triggerEventWithPosition != null)
        //     triggerEventWithPosition.OnEventRaised += (pos) => Play(pos, Quaternion.identity);
    }

    private void Unsubscribe()
    {
        if (triggerEvent != null)
            triggerEvent.OnPositionEventRaised -= Play;

        // if (triggerEventWithPosition != null)
        //     triggerEventWithPosition.OnEventRaised -= (pos) => Play(pos, Quaternion.identity);        
    }
}
