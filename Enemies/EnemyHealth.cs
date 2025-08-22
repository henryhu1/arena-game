using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemyComponent
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    private float currentHealth;
    private bool isDead = false;

    [Header("Events")]
    [SerializeField] private EnemyEventChannelSO deathEvent;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Awake()
    {
        currentHealth = stats.maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = stats.maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damagePoints, Vector3 fromPos, float force)
    {
        if (isDead) return;

        currentHealth -= damagePoints;
        isDead = currentHealth <= 0;

        Vector3 knockbackDirection = (transform.position - fromPos).normalized;
        controllerBase.BeDamaged(knockbackDirection, force, isDead);

        if (isDead)
        {
            deathEvent.RaiseEvent(controllerBase);
        }
    }

    public bool GetIsDead() { return isDead; }
}
