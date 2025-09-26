using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ParticleEffect
{
    public ParticleEffectSO particleData;
    public Vector3EventChannelSO triggerEvent;
}

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private List<ParticleEffect> effects;

    private readonly List<(Vector3EventChannelSO evt, UnityAction<Vector3> callback)> _subscriptions = new();

    private void Awake()
    {
    }

    private void OnEnable()
    {
        foreach (ParticleEffect effect in effects)
        {
            if (effect.triggerEvent != null)
            {
                void handler(Vector3 pos) => HandlePlay(effect, pos);
                effect.triggerEvent.OnPositionEventRaised += handler;
                _subscriptions.Add((effect.triggerEvent, handler));
            }
        }
    }

    private void OnDisable()
    {
        foreach (var (evt, handler) in _subscriptions)
        {
            evt.OnPositionEventRaised -= handler;
        }
        _subscriptions.Clear();
    }
 
    private void HandlePlay(ParticleEffect effect, Vector3 pos)
    {
        if (effect.particleData == null) return;

        ObjectPoolManager.Instance.Spawn(effect.particleData.particlePrefab, pos, effect.particleData.particlePrefab.transform.rotation);
    }
}
