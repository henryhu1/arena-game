using System.Collections;
using UnityEngine;

// TODO: projectile attacks
public class PlayerAttackController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private bool isAttacking;
    private bool isDamaging;

    private Coroutine attackingRoutine;

    private AttackType currentAttack;

    [Header("Hitbox Use")]
    [SerializeField] private PlayerHitbox hitbox;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO getWeaponEvent;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    void Start()
    {
        getWeaponEvent.OnWeaponEvent += UseWeapon;
    }

    void OnDestroy()
    {
        getWeaponEvent.OnWeaponEvent -= UseWeapon;
    }

    public void StartAttacking()
    {
        if (manager.health.GetIsDead() || attackingRoutine != null) return;

        isAttacking = true;
        attackingRoutine = StartCoroutine(DamageWindow(hitbox));
    }

    private void StopAttacking()
    {
        hitbox.EndAttack();
        isDamaging = false;
        isAttacking = false;
        StopCoroutine(attackingRoutine);
        attackingRoutine = null;
    }

    public bool GetIsAttacking()
    {
        return this.isAttacking;
    }

    public AttackType GetAttackType() { return currentAttack; }

    private void UseWeapon(WeaponData weaponData)
    {
        hitbox.ApplyWeaponData(weaponData);
        currentAttack = weaponData.attackType;
    }

    private void Update()
    {
        if (manager.IsControllerGrounded())
        {
            if (manager.movementController.GetIsJumpInitiated())
            {
                StopAttacking();
            }

            if (isAttacking)
            {
                // TODO: find a way to decouple animatorState from attack controller
                AnimatorStateInfo animatorState = manager.animationController.GetAnimatorState();
                // TODO: use events to end attack
                if (manager.animationController.IsAttackAnimation() && animatorState.normalizedTime >= 1f)
                {
                    StopAttacking();
                }
                if (isAttacking && !manager.movementController.GetMovement().Equals(Vector2.zero))
                {
                    StopAttacking();
                }
                if (isDamaging && manager.health.GetIsGettingDamaged())
                {
                    StopAttacking();
                }
            }
        }
    }

    private IEnumerator DamageWindow(PlayerHitbox hitbox)
    {
        float time = 0;
        AttackAnimationSO animation = manager.animationController.GetAttackAnimation(currentAttack);

        // Wait until the attack reaches the start time
        while (time < animation.attackStartTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        hitbox.StartAttack();
        isDamaging = true;

        // Wait until it reaches the end time
        while (time < animation.attackEndTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        StopAttacking();
    }
}
