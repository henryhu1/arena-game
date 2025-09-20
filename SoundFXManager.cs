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

    [Header("Unique Enemy Events")]
    [SerializeField] private EnemyEventChannelSO onEnemySpawned;
    [SerializeField] private EnemyEventChannelSO onEnemyAttack;
    [SerializeField] private EnemyEventChannelSO onEnemyDamaged;

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

        onEnemySpawned.OnEnemyEvent += PlaySpawnSound;
        onEnemyAttack.OnEnemyEvent += PlayAttackSound;
        onEnemyDamaged.OnEnemyEvent += PlayDamageSound;
    }

    private void OnDisable()
    {
        foreach (AudioEffect effect in effects)
        {
            if (effect.triggerEvent != null)
                effect.triggerEvent.OnPositionEventRaised -= pos => HandlePlay(effect, pos);
        }

        onEnemySpawned.OnEnemyEvent -= PlaySpawnSound;
        onEnemyAttack.OnEnemyEvent -= PlayAttackSound;
        onEnemyDamaged.OnEnemyEvent -= PlayDamageSound;
    }

    private void HandlePlay(AudioEffect effect, Vector3 pos)
    {
        if (effect.audioData == null) return;

        PlaySoundFXClip(effect.audioData.GetRandomClip(), pos, 1f);
    }

    public void PlaySpawnSound(EnemyControllerBase enemy)
    {
        PlaySound(enemy.GetSpawnSound());
    }

    public void PlayAttackSound(EnemyControllerBase enemy)
    {
        PlaySound(enemy.GetAttackSound());
    }

    public void PlayDamageSound(EnemyControllerBase enemy)
    {
        // PlaySound(enemy.GetEnemyStats().GetAudioProfile().damagedSound);
        if (PlayerManager.Instance.attackController.GetAttackType() != AttackType.BOW)
        {
            PlaySound(PlayerManager.Instance.GetAttackAudio());
        }
    }

    private void PlaySound(AudioEffectSO audioEffect)
    {
        if (audioEffect == null) return;
        AudioClip clip = audioEffect.GetRandomClip();
        if (clip == null) return;
        PlaySoundFXClip(clip, transform.position, 1);
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