using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioEffect
{
    public AudioEffectSO audioData;
    public Vector3EventChannelSO triggerEvent;
}

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private List<AudioEffect> effects;

    const int k_initialPoolSize = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        ObjectPoolManager.Instance.CreatePool(soundFXObject.gameObject, k_initialPoolSize);
    }

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

        PlaySoundFXClip(effect.audioData.GetRandomClip(), pos, 0.6f);
    }

    public void PlaySoundFXClip(AudioClip audioClip, Vector3 spawnPos, float volume)
    {
        GameObject audioGameObject = ObjectPoolManager.Instance.Spawn(soundFXObject.gameObject, spawnPos, Quaternion.identity);
        if (audioGameObject.TryGetComponent(out AudioSource audioSource))
        {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
        StartCoroutine(WaitToDespawnAudioSource(audioClip, audioGameObject));
    }

    private IEnumerator WaitToDespawnAudioSource(AudioClip audioClip, GameObject audioGameObject) {
        float clipLength = audioClip.length;
        yield return new WaitForSeconds(clipLength);

        ObjectPoolManager.Instance.Despawn(audioGameObject, soundFXObject.gameObject);
    }
}