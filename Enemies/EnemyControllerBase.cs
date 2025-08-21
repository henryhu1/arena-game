using System.Collections;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour, IPoolable
{
    [SerializeField] protected EnemyStats enemyStats;
    [SerializeField] protected EnemySpawnData spawnData;
    protected EnemyAnimation currentState;

    protected EnemyAI ai;
    protected EnemyHealth health;
    protected EnemyKnockback knockback;
    protected IEnemyAttackBehavior attack;

    public Animator animator;

    protected virtual void Awake()
    {
        ai = GetComponent<EnemyAI>();
        health = GetComponent<EnemyHealth>();
        knockback = GetComponent<EnemyKnockback>();
        attack = GetComponent<IEnemyAttackBehavior>();
        animator = GetComponent<Animator>();

        InitializeAll();
    }

    void InitializeAll()
    {
        ai.Initialize(this, enemyStats);
        health.Initialize(this, enemyStats);
        knockback.Initialize(this, enemyStats);
        attack.Initialize(this, enemyStats);
    }

    protected virtual void Update()
    {
        // if (ShouldAttack())
        //     attack?.Attack();
    }

    public void RestartAgent()
    {
        ai.EnableAgent();
    }

    public void DisableAgent()
    {
        ai.DisableAgent();
    }

    public void SetDamageState()
    {
        currentState = EnemyAnimation.Damage;
    }

    public void SetAttackState()
    {
        currentState = EnemyAnimation.Attack;
    }

    public virtual bool CanAttack()
    {
        return !knockback.GetIsStunned() &&
            !health.GetIsDead() &&
            currentState != EnemyAnimation.Damage &&
            currentState != EnemyAnimation.Death;
    }

    public void WarpAgent(Vector3 pos)
    {
        ai.WarpAgent(pos);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockback.ApplyKnockback(direction, force);
    }

    public void HandleDeath()
    {
        currentState = EnemyAnimation.Death;
        StartCoroutine(WaitForDeathAnimationAndDespawn());
    }

    private void OnEnable()
    {
        currentState = EnemyAnimation.Idle;
    }

    private IEnumerator WaitForDeathAnimationAndDespawn()
    {
        // Wait for the animation to finish based on its length
        yield return new WaitForSeconds(enemyStats.DeathClipLength);

        // Then despawn via pooling
        EnemySpawner.Instance.DespawnEnemy(gameObject, spawnData);
    }

    public void OnSpawned(Vector3 pos)
    {
        health.ResetHealth();
        ai.EnableAgent();
        ai.WarpAgent(pos);
    }

    public void OnDespawned()
    {

    }

    public EnemyStats GetEnemyStats()
    {
        return enemyStats;
    }

    public EnemySpawnData GetSpawnData()
    {
        return spawnData;
    }
}