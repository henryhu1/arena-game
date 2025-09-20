using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemyComponent
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    private float currentHealth;
    private bool isDead = false;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO onEnemyHit;
    [SerializeField] private EnemyEventChannelSO onEnemyDamaged;
    [SerializeField] private EnemyEventChannelSO onEnemyDefeated;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Awake()
    {
        currentHealth = stats.MaxHealth();
    }

    public void ResetHealth()
    {
        currentHealth = stats.MaxHealth();
        isDead = false;
    }

    public void TakeDamage(Vector3 contactPos, float damage, Vector3 fromPos, float force)
    {
        if (isDead || controllerBase.GetIsStunned()) return;

        currentHealth -= damage;
        isDead = currentHealth <= 0;

        onEnemyHit.RaiseEvent(contactPos);
        onEnemyDamaged.RaiseEvent(controllerBase);

        Vector3 knockbackDirection = (transform.position - fromPos).normalized;
        controllerBase.BeDamaged(knockbackDirection, force, isDead);

        if (isDead)
        {
            onEnemyDefeated.RaiseEvent(controllerBase);
            controllerBase.DecrementAliveCount();
        }
    }

    public bool GetIsDead() { return isDead; }
}
