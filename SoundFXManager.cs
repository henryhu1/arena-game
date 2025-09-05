using System.Collections;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;

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