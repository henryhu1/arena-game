using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(IEnemyAttackBehavior))]
[RequireComponent(typeof(EnemyKnockback))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyControllerBase : MonoBehaviour, IPoolable, IHittable
{
    [Header("Data")]
    [SerializeField] protected EnemyStats enemyStats;
    [SerializeField] protected EnemySpawnData spawnData;

    [Header("Stat Multiplier")]
    [SerializeField] protected float sizeMultiplier = 1;
    [SerializeField] protected float sizeMultiplierMin = 1;
    [SerializeField] protected float sizeMultiplierMax = 1;

    protected Vector3 originalScale;

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

        originalScale = transform.localScale;

        ScaleEnemy();

        InitializeAll();
    }

    protected virtual void InitializeAll()
    {
        ai.Initialize(this);
        health.Initialize(this);
        knockback.Initialize(this);
        attack.Initialize(this);
    }

    protected virtual void Update()
    {
        if (ai.IsAgentEnabled())
        {
            if (!ai.IsFollowingPlayer() && ai.IsDestinationReached())
            {
                currentState = EnemyAnimation.Idle;
            }
            else
            {
                currentState = EnemyAnimation.Walk;
            }
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
            ai.IsFollowingPlayer() &&
            currentState != EnemyAnimation.Damage &&
            currentState != EnemyAnimation.Death;
    }

    public Vector3 WarpAgent(Vector3 pos)
    {
        return ai.WarpAgent(pos);
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

    // TODO: should be levaraged more, not just for proejctiles
    public bool TakeHit(Vector3 contactPos, float damagePoints, Vector3 fromDirection, float force)
    {
        if (knockback.IsKnockbackableByProjectile())
        {
            health.TakeDamage(contactPos, damagePoints, fromDirection, force);
            return true;
        }
        else
        {
            knockback.ApplyKnockback(fromDirection, force);
            if (knockedBackStunBuffer != null)
            {
                StopCoroutine(knockedBackStunBuffer);
            }
            knockedBackStunBuffer = StartCoroutine(KnockbackRoutine());
            DisableAgent(EnemyAnimation.Damage);
            return false;
        }
    }

    private void OnEnable()
    {
        currentState = EnemyAnimation.Idle;
    }

    private IEnumerator WaitForAttackAnimation()
    {
        while (!IsAnimationFinished(EnemyAnimation.Attack))
        {
            yield return null;
        }
        RestartAgent();
    }

    private IEnumerator KnockbackRoutine()
    {
        isStunned = true;
        float elapsedTime = 0;

        while (!IsAnimationFinished(EnemyAnimation.Damage))
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(GetKnockbackTime() - elapsedTime);

        RestartAgent();
        knockedBackStunBuffer = null;
        isStunned = false;
    }

    private IEnumerator WaitForDeathAnimationAndDespawn()
    {
        while (!IsAnimationFinished(EnemyAnimation.Death))
        {
            yield return null;
        }
        Despawn();
    }

    private bool IsAnimationFinished(EnemyAnimation animationName)
    {
        return IsAnimationPlaying(animationName) && GetAnimatorNormalizedTime() >= 1;
    }

    public bool IsAnimationPlaying(EnemyAnimation animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(enemyStats.GetAnimationName(animationName));
    }

    public float GetAnimatorNormalizedTime()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }

    protected void ScaleEnemy()
    {
        animator.speed = 1 / sizeMultiplier;
        transform.localScale = originalScale * sizeMultiplier;
    }

    private void Despawn()
    {
        EnemySpawner.Instance.DespawnEnemy(gameObject, spawnData);
    }

    public virtual void OnSpawned(Vector3 pos)
    {
        ScaleEnemy();
        health.ResetHealth();
        attack.Setup();
        ai.ResetAgent(pos);
    }

    public void OnDespawned()
    {
        currentState = EnemyAnimation.Idle;
    }

    public void DecrementAliveCount() { spawnData.currentAlive--; }

    public float GetPointValue() { return enemyStats.pointValue * sizeMultiplier; }
    public float GetTimeRegained() { return enemyStats.timeRegained * sizeMultiplier; }
    public float GetMaxHealth() { return enemyStats.maxHealth * sizeMultiplier; }
    public float GetDamage() { return enemyStats.damage * sizeMultiplier; }
    public float GetAttackRange() { return enemyStats.attackRange * sizeMultiplier; }
    public float GetAttackStart() { return enemyStats.attackStart; }
    public float GetAttackEnd() { return enemyStats.attackEnd; }
    public float GetAttackCooldown() { return enemyStats.attackCooldown; }
    public float GetMoveSpeed() { return enemyStats.moveSpeed / sizeMultiplier; }
    public float GetKnockbackDistance() { return enemyStats.knockbackDistance * sizeMultiplier; }
    protected float GetKnockbackTime() { return enemyStats.knockbackTime * sizeMultiplier; }
    public AudioEffectSO GetSpawnSound() { return enemyStats.audioProfile.spawnSound; }
    public AudioEffectSO GetAttackSound() { return enemyStats.audioProfile.attackSound; }
    public EnemySpawnStrategy GetSpawnStrategy() { return spawnData.spawnStrategy; }
    public bool GetIsStunned() { return isStunned; }
}
