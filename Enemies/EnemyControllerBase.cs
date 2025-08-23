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

    protected Animator animator;
    private Coroutine animationCoroutine;
    protected Coroutine knockedBackStunBuffer;
    protected bool isStunned;

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
        if (ai.IsAgentEnabled() && currentState == EnemyAnimation.Idle)
        {
            currentState = EnemyAnimation.Walk;
        }

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        string animationName = enemyStats.GetAnimationName(currentState);

        if (!animatorState.IsName(animationName))
        {
            if (currentState == EnemyAnimation.Walk && ai.IsAgentEnabled())
            {
                animator.Play(animationName, 0, Random.value);
            }
            else animator.Play(animationName);
        }
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
        return !isStunned &&
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
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(WaitForAttackAnimation());
    }

    public void BeDamaged(Vector3 direction, float force, bool isDead)
    {
        transform.rotation = Quaternion.LookRotation(-direction);

        EnemyAnimation nextState;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        if (isDead)
        {
            nextState = EnemyAnimation.Death;
            animationCoroutine = StartCoroutine(WaitForDeathAnimationAndDespawn());
        }
        else
        {
            nextState = EnemyAnimation.Damage;
            knockback.ApplyKnockback(direction, force);
            if (knockedBackStunBuffer != null)
            {
                StopCoroutine(knockedBackStunBuffer);
            }
            knockedBackStunBuffer = StartCoroutine(KnockbackRoutine());
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

    private IEnumerator KnockbackRoutine()
    {
        isStunned = true;
        // TODO: formula for knockback time?
        //   Also consolidate knockback time and damage animation time
        float bufferTime = Mathf.Max(enemyStats.AttackClipLength, enemyStats.knockbackTime);
        yield return new WaitForSeconds(bufferTime);

        RestartAgent();
        knockedBackStunBuffer = null;
        isStunned = false;
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