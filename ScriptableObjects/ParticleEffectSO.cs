using UnityEngine;

[CreateAssetMenu(fileName = "ParticleEffectSO", menuName = "Particles/Effects")]
public class ParticleEffectSO : ScriptableObject
{
    public GameObject particlePrefab;
    public int initialPoolSize = 2;
}
