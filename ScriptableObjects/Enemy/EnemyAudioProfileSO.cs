using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAudioProfileSO", menuName = "Enemy/AudioProfile")]
public class EnemyAudioProfileSO : ScriptableObject
{
    public AudioEffectSO spawnSound;
    public AudioEffectSO attackSound;
    public AudioEffectSO damagedSound;
    public AudioEffectSO despawnSound;
}
