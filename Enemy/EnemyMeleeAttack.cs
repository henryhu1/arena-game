using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttackBehavior
{
    private EnemyControllerBase controllerBase;

    [Header("Hitbox Use")]
    [SerializeField] private EnemyHitbox hitbox;

    [Header("Events")]
    [SerializeField] private EnemyEventChannelSO attackEvent;
    [SerializeField] private EnemyEventChannelSO damagedEvent;

    private bool isAttacking;
    private float attackCooldownTimer;

    const float k_enemyReach = 1.3f;

    private Coroutine attackingCoroutine;

    public void Initialize(EnemyControllerBase controllerBase)
    {
        this.controllerBase = controllerBase;
    }

    private void OnEnable()
    {
        damagedEvent.OnEnemyEvent += DamagedEvent_CancelAttack;
    }

    private void OnDisable()
    {
        damagedEvent.OnEnemyEvent -= DamagedEvent_CancelAttack;
    }

    private void Update()
    {
        if (PlayerManager.Instance == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);
        attackCooldownTimer -= Time.deltaTime;

        if (distanceToPlayer <= controllerBase.GetAttackRange() * k_enemyReach &&
            attackCooldownTimer <= 0f &&
            !isAttacking &&
            controllerBase.CanAttack())
        {
            attackingCoroutine = StartCoroutine(PerformAttack());
        }
    }

    public void Setup()
    {
        hitbox.Setup(controllerBase.GetDamage(), controllerBase.GetAttackRange());
    }

    public IEnumerator PerformAttack()
    {
        isAttacking = true;
        controllerBase.SetAttackState();

        while (!controllerBase.IsAnimationPlaying(EnemyAnimation.Attack))
        {
            yield return null;
        }

        float animationTime = controllerBase.GetAnimatorNormalizedTime();
        while (animationTime < controllerBase.GetAttackStart())
        {
            animationTime = controllerBase.GetAnimatorNormalizedTime();
            yield return null;
        }

        attackEvent.RaiseEvent(controllerBase);
        hitbox.StartAttack();

        while (animationTime < controllerBase.GetAttackEnd())
        {
            animationTime = controllerBase.GetAnimatorNormalizedTime();
            yield return null;
        }

        hitbox.EndAttack();
        attackCooldownTimer = controllerBase.GetAttackCooldown();
        isAttacking = false;
    }

    private void DamagedEvent_CancelAttack(EnemyControllerBase _)
    {
        if (attackingCoroutine != null)
        {
            StopCoroutine(attackingCoroutine);
        }
        hitbox.EndAttack();
        isAttacking = false;
    }
}