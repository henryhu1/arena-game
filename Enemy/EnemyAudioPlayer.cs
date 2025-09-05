using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioPlayer : MonoBehaviour, IEnemyComponent
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    private AudioSource audioSource;

    [SerializeField] private EnemyAudioProfileSO audioProfile;

    [Header("Events")]
    [SerializeField] private EnemyEventChannelSO onEnemySpawned;
    [SerializeField] private EnemyEventChannelSO onEnemyAttack;
    [SerializeField] private EnemyEventChannelSO onEnemyDamaged;
    [SerializeField] private Vector3EventChannelSO onEnemyDespawn;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        onEnemySpawned.OnEnemyEvent += PlaySpawnSound;
        onEnemyAttack.OnEnemyEvent += PlayAttackSound;
        onEnemyDamaged.OnEnemyEvent += PlayDamageSound;
        onEnemyDespawn.OnPositionEventRaised += PlayDeathSound;
    }

    private void OnDisable()
    {
        onEnemySpawned.OnEnemyEvent -= PlaySpawnSound;
        onEnemyAttack.OnEnemyEvent -= PlayAttackSound;
        onEnemyDamaged.OnEnemyEvent -= PlayDamageSound;
        onEnemyDespawn.OnPositionEventRaised -= PlayDeathSound;
    }

    public void PlaySpawnSound(EnemyControllerBase _) => PlaySound(audioProfile.spawnSound);
    public void PlayAttackSound(EnemyControllerBase _) => PlaySound(audioProfile.attackSound);
    public void PlayDamageSound(EnemyControllerBase _) => PlaySound(audioProfile.damagedSound);
    public void PlayDeathSound(Vector3 _) => PlaySound(audioProfile.despawnSound);

    private void PlaySound(AudioEffectSO audioEffect)
    {
        if (audioEffect == null) return;
        AudioClip clip = audioEffect.GetRandomClip();
        if (clip == null) return;

        audioSource.pitch = audioEffect.GetRandomPitch();
        audioSource.volume = audioEffect.volume;
        audioSource.PlayOneShot(clip);
    }
}
