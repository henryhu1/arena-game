using System.Collections;
using UnityEngine;

// TODO: projectile attacks
public class PlayerAttackController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CharacterController controller;

    private bool isAttacking;
    private bool isDamaging;
    private AttackType currentAttack;

    [Header("Hitbox Use")]
    [SerializeField] private PlayerHitbox hitbox;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO getWeaponEvent;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentAttack = AttackType.MELEE;
    }

    void Start()
    {
        getWeaponEvent.OnWeaponEvent += UseWeapon;
    }

    void OnDestroy()
    {
        getWeaponEvent.OnWeaponEvent -= UseWeapon;
    }

    public bool SetIsAttacking(bool isAttacking)
    {
        return this.isAttacking = isAttacking;
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
        if (controller.isGrounded)
        {
            if (manager.movementController.GetIsJumpInitiated())
            {
                isAttacking = false;
            }

            if (isAttacking)
            {
                StartCoroutine(DamageWindow(hitbox));

                // TODO: find a way to decouple animatorState from attack controller
                AnimatorStateInfo animatorState = manager.animationController.GetAnimatorState();
                // TODO: use events to end attack
                if (manager.animationController.IsAttackAnimation() && animatorState.normalizedTime >= 1f)
                {
                    isAttacking = false;
                }
                if (isAttacking && !manager.movementController.GetMovement().Equals(Vector2.zero))
                {
                    isAttacking = false;
                }
                if (isDamaging && manager.health.GetIsGettingDamaged())
                {
                    isAttacking = false;
                    isDamaging = false;
                }
            }
            else
            {
                hitbox.EndAttack();
            }
        }
    }

    private IEnumerator DamageWindow(PlayerHitbox hitbox)
    {
        AnimatorStateInfo stateInfo = manager.animationController.GetAnimatorState();
        AttackAnimationSO animation = manager.animationController.GetAttackAnimation(currentAttack);

        // Wait until the attack reaches the start time
        while (stateInfo.normalizedTime < animation.attackStartTime)
            yield return null;

        hitbox.StartAttack();
        isDamaging = true;

        // Wait until it reaches the end time
        while (stateInfo.normalizedTime < animation.attackEndTime)
            yield return null;

        hitbox.EndAttack();
        isDamaging = false;
    }
}
