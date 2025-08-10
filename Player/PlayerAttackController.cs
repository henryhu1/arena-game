using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CharacterController controller;

    private bool isAttacking;
    private bool isDamaging;

    [Header("Hitbox Use")]
    [SerializeField] private PlayerHitbox hitbox;
    [SerializeField] private GameObject gripPoint;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public bool SetIsAttacking(bool isAttacking)
    {
        return this.isAttacking = isAttacking;
    }

    public bool GetIsAttacking()
    {
        return this.isAttacking;
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

                AnimatorStateInfo animatorState = manager.animationController.GetAnimatorState();
                if (animatorState.IsName(PlayerAnimations.PUNCH_LEFT.GetAnimationName()) && animatorState.normalizedTime >= 1f)
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

        // Wait until the attack reaches the start time
        while (stateInfo.normalizedTime < hitbox.GetDamageStartTime())
            yield return null;

        hitbox.StartAttack();
        isDamaging = true;

        // Wait until it reaches the end time
        while (stateInfo.normalizedTime < hitbox.GetDamageEndTime())
            yield return null;

        hitbox.EndAttack();
        isDamaging = false;
    }

}
