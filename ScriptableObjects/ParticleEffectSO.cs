using UnityEngine;

[CreateAssetMenu(fileName = "ParticleEffectSO", menuName = "Effects/Particle")]
public class ParticleEffectSO : ScriptableObject
{
    public GameObject particlePrefab;
    public int initialPoolSize = 2;
}
