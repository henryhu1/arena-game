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

    public void TakeDamage(float damagePoints, Vector3 playerPos, float force)
    {
        if (isDead) return;

        currentHealth -= damagePoints;

        if (currentHealth <= 0)
        {
            isDead = true;
            controllerBase.HandleDeath();
            deathEvent.RaiseEvent(controllerBase);
        }
        else
        {
            Vector3 knockbackDirection = (transform.position - playerPos).normalized;
            controllerBase.ApplyKnockback(knockbackDirection, force);
        }
    }

    public bool GetIsDead() { return isDead; }
}
