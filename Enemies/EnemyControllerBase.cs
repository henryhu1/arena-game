using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour
{
    public EnemyStats enemyStats;

    protected EnemyAI ai;
    protected EnemyHealth health;
    protected IEnemyKnockbackable knockback;
    protected IAttackBehavior attack;

    protected virtual void Awake()
    {
        ai = GetComponent<EnemyAI>();

        health = GetComponent<EnemyHealth>();

        knockback = GetComponent<IEnemyKnockbackable>();

        attack = GetComponent<IAttackBehavior>();

        InitializeAll();
    }

    void InitializeAll()
    {
        ai.Initialize(this, enemyStats);
        health.Initialize(this, enemyStats);
        knockback.Initialize(this, enemyStats);
    }

    protected virtual void Update()
    {
        if (ShouldAttack())
            attack?.Attack();
    }

    protected abstract bool ShouldAttack();

    public abstract void RestartAgent();
    public abstract void DisableAgent();

    public abstract void SetDamageState();

    public abstract void WarpAgent(Vector3 pos, float distanceRange);
}