using System.Collections;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour, IPoolable
{
    public EnemyStats enemyStats;
    public EnemySpawnData spawnData;

    protected EnemyAI ai;
    protected EnemyHealth health;
    protected EnemyKnockback knockback;
    protected IEnemyAttackBehavior attack;

    protected virtual void Awake()
    {
        ai = GetComponent<EnemyAI>();

        health = GetComponent<EnemyHealth>();

        knockback = GetComponent<EnemyKnockback>();

        attack = GetComponent<IEnemyAttackBehavior>();

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

    public abstract void RestartAgent();
    public abstract void DisableAgent();

    public abstract void SetDamageState();
    public abstract void SetAttackState();
    public virtual bool CanAttack() { return !knockback.GetIsStunned(); }

    public abstract void WarpAgent(Vector3 pos);

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockback.ApplyKnockback(direction, force);
    }

    public virtual void HandleDeath()
    {
        StartCoroutine(WaitForDeathAnimationAndDespawn());
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
}