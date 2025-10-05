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

    const float k_checkSphereBuffer = 2.2f;

    private Collider[] hitColliders;
    private int playerLayerMask;
    private float attackRange;
    private Coroutine attackingCoroutine;

    public void Initialize(EnemyControllerBase controllerBase)
    {
        this.controllerBase = controllerBase;
    }

    private void Start()
    {
        hitColliders = new Collider[1];
        playerLayerMask = LayerMask.GetMask("Player");
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
        attackCooldownTimer -= Time.deltaTime;

        float checkDist = attackRange / k_checkSphereBuffer;
        int hits = Physics.OverlapSphereNonAlloc(
            transform.position + (transform.forward + Vector3.up) * checkDist,
            checkDist,
            hitColliders,
            playerLayerMask
        );

        if (hits > 0 &&
            attackCooldownTimer <= 0f &&
            !isAttacking &&
            controllerBase.CanAttack() &&
            attackingCoroutine == null)
        {
            attackingCoroutine = StartCoroutine(PerformAttack());
        }
    }

    public void Setup()
    {
        hitbox.Setup(controllerBase.GetDamage(), controllerBase.GetAttackRange());
        attackRange = controllerBase.GetAttackRange();
    }

    public IEnumerator PerformAttack()
    {
        isAttacking = true;
        controllerBase.SetAttackState();

        yield return new WaitUntil(() => controllerBase.IsAnimationPlaying(EnemyAnimation.Attack));

        yield return new WaitUntil(() => {
            float animationTime = controllerBase.GetAnimatorNormalizedTime();
            return animationTime >= controllerBase.GetAttackStart();
        });

        attackEvent.RaiseEvent(controllerBase);
        hitbox.StartAttack();

        yield return new WaitUntil(() => {
            float animationTime = controllerBase.GetAnimatorNormalizedTime();
            return animationTime >= controllerBase.GetAttackEnd();
        });

        hitbox.EndAttack();
        attackCooldownTimer = controllerBase.GetAttackCooldown();
        isAttacking = false;
        attackingCoroutine = null;
    }

    private void DamagedEvent_CancelAttack(EnemyControllerBase _)
    {
        if (attackingCoroutine != null)
        {
            StopCoroutine(attackingCoroutine);
            attackingCoroutine = null;
        }
        hitbox.EndAttack();
        isAttacking = false;
    }
}