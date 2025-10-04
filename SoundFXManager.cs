using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Item Events")]
    [SerializeField] private WeaponEventChannelSO onWeaponEquip;
    [SerializeField] private WeaponEventChannelSO onWeaponDrop;

    [Header("Item Audio")]
    [SerializeField] private AudioEffectSO weaponEquipEffect;
    [SerializeField] private AudioEffectSO weaponDropEffect;

    const int k_initialPoolSize = 10;
    private readonly List<(Vector3EventChannelSO evt, UnityAction<Vector3> callback)> _subscriptions = new();

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
            {
                void handler(Vector3 pos) => HandlePlay(effect, pos);
                effect.triggerEvent.OnPositionEventRaised += handler;
                _subscriptions.Add((effect.triggerEvent, handler));
            }
        }

        onEnemySpawned.OnEnemyEvent += PlaySpawnSound;
        onEnemyAttack.OnEnemyEvent += PlayAttackSound;
        onEnemyDamaged.OnEnemyEvent += PlayDamageSound;

        onWeaponEquip.OnWeaponEvent += PlayWeaponEquipSound;
        onWeaponDrop.OnWeaponEvent += PlayWeaponDropSound;
    }

    private void OnDisable()
    {
        foreach (var (evt, handler) in _subscriptions)
        {
            evt.OnPositionEventRaised -= handler;
        }
        _subscriptions.Clear();

        onEnemySpawned.OnEnemyEvent -= PlaySpawnSound;
        onEnemyAttack.OnEnemyEvent -= PlayAttackSound;
        onEnemyDamaged.OnEnemyEvent -= PlayDamageSound;

        onWeaponEquip.OnWeaponEvent -= PlayWeaponEquipSound;
        onWeaponDrop.OnWeaponEvent -= PlayWeaponDropSound;
    }

    private void HandlePlay(AudioEffect effect, Vector3 pos)
    {
        if (effect.audioData == null) return;

        PlaySoundFXClip(effect.audioData.GetRandomClip(), pos, 1f);
    }

    public void PlaySpawnSound(EnemyControllerBase enemy)
    {
        PlaySound(enemy.GetSpawnSound(), enemy.transform.position);
    }

    public void PlayAttackSound(EnemyControllerBase enemy)
    {
        PlaySound(enemy.GetAttackSound(), enemy.transform.position);
    }

    public void PlayDamageSound(EnemyControllerBase enemy)
    {
        // PlaySound(enemy.GetEnemyStats().GetAudioProfile().damagedSound);
        PlaySound(PlayerManager.Instance.GetAttackAudio(), enemy.transform.position);
    }

    public void PlayWeaponEquipSound(Weapon weapon)
    {
        PlaySound(weaponEquipEffect, weapon.transform.position);
    }

    public void PlayWeaponDropSound(Weapon weapon)
    {
        PlaySound(weaponDropEffect, weapon.transform.position);
    }

    private void PlaySound(AudioEffectSO audioEffect, Vector3 pos)
    {
        if (audioEffect == null) return;
        AudioClip clip = audioEffect.GetRandomClip();
        if (clip == null) return;
        PlaySoundFXClip(clip, pos, 1);
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