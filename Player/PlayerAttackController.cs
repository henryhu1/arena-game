using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private bool isAttacking;
    private bool isDamaging;

    private Coroutine attackingRoutine;


    [Header("Audio")]
    [SerializeField] private AudioEffectSO fistAudio;

    [Header("Hitbox Use")]
    [SerializeField] private PlayerHitbox hitbox;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
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
        WeaponData heldWeaponData = manager.inventoryHandler.GetHeldWeaponData();
        if (heldWeaponData == null)
        {
            return AttackType.MELEE;
        }

        return heldWeaponData.attackType;
    }

    public AudioEffectSO GetAttackAudio()
    {
        WeaponData heldWeaponData = manager.inventoryHandler.GetHeldWeaponData();
        if (heldWeaponData == null)
        {
            return fistAudio;
        }

        return heldWeaponData.contactAudio;
    }

    public void UseWeapon(Weapon weapon)
    {
        hitbox.ApplyWeaponData(weapon);
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
        WeaponData heldWeaponData = manager.inventoryHandler.GetHeldWeaponData();

        float time = 0;
        AttackAnimationSO animation;
        if (heldWeaponData != null)
        {
            animation = manager.animationController.GetAttackAnimation(heldWeaponData.attackType);
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

        if (heldWeaponData is BowData bowData)
        {
            if (manager.inventoryHandler.ConsumeArrow())
            {
                bowData.FireArrow(manager.projectileSpawnPoint.transform);
            }
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
