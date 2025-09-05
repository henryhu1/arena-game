using UnityEngine;

[System.Serializable]
public class AudioEffect
{
    public AudioEffectSO audioData;
    public Vector3EventChannelSO triggerEvent;
}

public class AudioEffectListener : MonoBehaviour
{
    [SerializeField] AudioEffect[] effects;

    private void OnEnable()
    {
        foreach (AudioEffect effect in effects)
        {
            if (effect.triggerEvent != null)
                effect.triggerEvent.OnPositionEventRaised += pos => HandlePlay(effect, pos);
        }
    }

    private void OnDisable()
    {
        foreach (AudioEffect effect in effects)
        {
            if (effect.triggerEvent != null)
                effect.triggerEvent.OnPositionEventRaised -= pos => HandlePlay(effect, pos);
        }
    }

    private void HandlePlay(AudioEffect effect, Vector3 pos)
    {
        if (effect.audioData == null) return;

        SoundFXManager.Instance.PlaySoundFXClip(effect.audioData.GetRandomClip(), pos, 1);
    }
}
