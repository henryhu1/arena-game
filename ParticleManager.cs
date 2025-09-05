using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleEffect
{
    public ParticleEffectSO particleData;
    public Vector3EventChannelSO triggerEvent;
}

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private List<ParticleEffect> effects;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        foreach (ParticleEffect effect in effects)
        {
            if (effect.triggerEvent != null)
                effect.triggerEvent.OnPositionEventRaised += pos => HandlePlay(effect, pos);
        }
    }

    private void OnDisable()
    {
        foreach (ParticleEffect effect in effects)
        {
            if (effect.triggerEvent != null)
                effect.triggerEvent.OnPositionEventRaised -= pos => HandlePlay(effect, pos);
        }
    }
 
    private void HandlePlay(ParticleEffect effect, Vector3 pos)
    {
        if (effect.particleData == null) return;

        ObjectPoolManager.Instance.Spawn(effect.particleData.particlePrefab, pos, effect.particleData.particlePrefab.transform.rotation);
    }
}
