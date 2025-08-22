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
        currentState = EnemyAnimation.Walk;
        ai.EnableAgent();
    }

    public void DisableAgent(EnemyAnimation nextState)
    {
        currentState = nextState;
        ai.DisableAgent();
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

    public void SetAttackState()
    {
        DisableAgent(EnemyAnimation.Attack);
        StartCoroutine(WaitForAttackAnimation());
    }

    public void BeDamaged(Vector3 direction, float force, bool isDead)
    {
        EnemyAnimation nextState;
        if (isDead)
        {
            nextState = EnemyAnimation.Death;
            StartCoroutine(WaitForDeathAnimationAndDespawn());
        }
        else
        {
            nextState = EnemyAnimation.Damage;
            StartCoroutine(WaitForDamageAnimation());
            knockback.ApplyKnockback(direction, force);
        }
        DisableAgent(nextState);
    }

    private void OnEnable()
    {
        currentState = EnemyAnimation.Idle;
    }

    private IEnumerator WaitForAttackAnimation()
    {
        yield return new WaitForSeconds(enemyStats.AttackClipLength);
        RestartAgent();
    }

    private IEnumerator WaitForDamageAnimation()
    {
        yield return new WaitForSeconds(enemyStats.DamageClipLength);
    }

    private IEnumerator WaitForDeathAnimationAndDespawn()
    {
        yield return new WaitForSeconds(enemyStats.DeathClipLength);
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
        currentState = EnemyAnimation.Idle;
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