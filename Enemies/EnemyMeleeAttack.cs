using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttackBehavior
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    [Header("Hitbox Use")]
    [SerializeField] private EnemyHitbox hitbox;

    [Header("Events")]
    [SerializeField] private EnemyEventChannelSO damagedEvent;

    private bool isAttacking;
    private float attackCooldownTimer;

    const float k_enemyReach = 1.2f;

    private Coroutine attackingCoroutine;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Start()
    {
        hitbox.Setup(stats.Damage(), stats.AttackRange());
        damagedEvent.OnEnemyEvent += DamagedEvent_CancelAttack;
    }

    private void OnDestroy()
    {
        damagedEvent.OnEnemyEvent -= DamagedEvent_CancelAttack;
    }

    private void Update()
    {
        if (PlayerManager.Instance == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.position);
        attackCooldownTimer -= Time.deltaTime;

        if (distanceToPlayer <= stats.AttackRange() * k_enemyReach &&
            attackCooldownTimer <= 0f &&
            !isAttacking &&
            controllerBase.CanAttack())
        {
            attackingCoroutine = StartCoroutine(PerformAttack());
        }
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
        while (animationTime < stats.AttackStart())
        {
            animationTime = controllerBase.GetAnimatorNormalizedTime();
            yield return null;
        }

        hitbox.StartAttack();

        while (animationTime < stats.AttackEnd())
        {
            animationTime = controllerBase.GetAnimatorNormalizedTime();
            yield return null;
        }

        hitbox.EndAttack();
        attackCooldownTimer = stats.AttackCooldown();
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