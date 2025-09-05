using UnityEngine;

[CreateAssetMenu(fileName = "AudioEffectSO", menuName = "Audio")]
public class AudioEffectSO : ScriptableObject
{
    public AudioClip[] clips;
    public float volume = 1f;
    public float pitchMin = 1f;
    public float pitchMax = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    public float GetRandomPitch() => Random.Range(pitchMin, pitchMax);
}
