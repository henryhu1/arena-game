using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(IEnemyAttackBehavior))]
[RequireComponent(typeof(EnemyKnockback))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyControllerBase : MonoBehaviour, IPoolable
{
    [SerializeField] protected EnemyStats enemyStats;
    [SerializeField] protected EnemySpawnData spawnData;
    private Vector3 originalScale;

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

        yield return new WaitForSeconds(enemyStats.KnockbackTime() - elapsedTime);

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
        EnemySpawner.Instance.DespawnEnemy(gameObject, spawnData);
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

    private void ScaleEnemy()
    {
        animator.speed = 1 / enemyStats.sizeMultiplier;
        transform.localScale = originalScale * enemyStats.sizeMultiplier;
    }

    public virtual void OnSpawned(Vector3 pos)
    {
        ScaleEnemy();
        health.ResetHealth();
        ai.ResetAgent(pos);
    }

    public void OnDespawned()
    {
        currentState = EnemyAnimation.Idle;
    }

    public EnemyStats GetEnemyStats() { return enemyStats; }

    public EnemySpawnData GetSpawnData() { return spawnData; }

    public bool GetIsStunned() { return isStunned; }
}
