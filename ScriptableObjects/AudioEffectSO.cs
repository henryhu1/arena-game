using UnityEngine;

[CreateAssetMenu(fileName = "AudioEffectSO", menuName = "Effects/Audio")]
public class AudioEffectSO : ScriptableObject
{
    public float volume = 1f;

    public AudioClip[] clips;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
