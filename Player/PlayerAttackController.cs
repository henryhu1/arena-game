using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private bool isAttacking;
    private bool isDamaging;

    private Coroutine attackingRoutine;

    private WeaponData holdingWeapon;

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
        if (!isAttacking) return;

        hitbox.EndAttack();
        isDamaging = false;
        isAttacking = false;
        if (attackingRoutine != null)
        {
            StopCoroutine(attackingRoutine);
            attackingRoutine = null;
        }
    }

    public bool GetIsAttacking()
    {
        return this.isAttacking;
    }

    public AttackType GetAttackType()
    {
        if (holdingWeapon == null)
        {
            return AttackType.MELEE;
        }

        return holdingWeapon.attackType;
    }

    private void UseWeapon(WeaponData weaponData, GameObject impactPoint)
    {
        hitbox.ApplyWeaponData(weaponData, impactPoint);
        holdingWeapon = weaponData;
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
        AttackAnimationSO animation;
        if (holdingWeapon != null)
        {
            animation = manager.animationController.GetAttackAnimation(holdingWeapon.attackType);
        }
        else
        {
            animation = manager.animationController.GetAttackAnimation(AttackType.MELEE);
        }

        while (time < animation.attackStartTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if (holdingWeapon is BowData bowData)
        {
            bowData.FireArrow(manager.projectileSpawnPoint.transform);
        }
        else
        {
            hitbox.StartAttack();
            isDamaging = true;
        }

        while (time < animation.attackEndTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        StopAttacking();
    }
}
