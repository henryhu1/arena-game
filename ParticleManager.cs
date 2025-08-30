using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private List<ParticleEffectSO> effects;

    private void Awake()
    {
        effects.ForEach(effect => effect.isPoolInitialized = false);
    }

    private void OnEnable()
    {
        foreach (var effect in effects)
            effect.InitializePoolIfNeeded();
    }

    private void OnDisable()
    {
        foreach (var effect in effects)
            effect.Dispose();
    }
}
