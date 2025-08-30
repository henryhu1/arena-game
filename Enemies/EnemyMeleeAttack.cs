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

        if (distanceToPlayer <= stats.AttackRange() &&
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

        yield return new WaitForSeconds(stats.AttackStart());

        // Damage window starts
        float damageWindow = stats.AttackEnd() - stats.AttackStart();
        float elapsed = 0f;
        hitbox.StartAttack();

        while (elapsed < damageWindow)
        {
            elapsed += Time.deltaTime;
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