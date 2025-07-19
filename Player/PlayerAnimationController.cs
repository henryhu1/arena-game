using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CharacterController controller;
    [SerializeField] private Animator animator;

    private PlayerAnimations currentState;
    const float k_animationCrossFade = 0.1f;
    const float k_jumpChangeInDirectionThreshold = 0.1f;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public AnimatorStateInfo GetAnimatorState()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }

    private void Update()
    {
        Vector2 movement = manager.movementController.GetMovement();
        Vector3 velocity = manager.movementController.GetVelocity();
        if (controller.isGrounded)
        {
            if (velocity.y <= 0f)
            {
                if (manager.movementController.GetIsJumping() || manager.movementController.GetIsFalling())
                {
                    return;
                }
                else if (movement.y > 0)
                {
                    if (movement.x < 0)
                        ChangeAnimationState(PlayerAnimations.RUN_RIGHT);
                    else if (movement.x > 0)
                        ChangeAnimationState(PlayerAnimations.RUN_LEFT);
                    else if (manager.movementController.GetIsSprinting())
                        ChangeAnimationState(PlayerAnimations.SPRINT);
                    else
                        ChangeAnimationState(PlayerAnimations.RUN_FORWARD);
                }
                else if (movement.y < 0)
                {
                    if (movement.x < 0)
                        ChangeAnimationState(PlayerAnimations.RUN_BACKWARD_LEFT);
                    else if (movement.x > 0)
                        ChangeAnimationState(PlayerAnimations.RUN_BACKWARD_RIGHT);
                    else
                        ChangeAnimationState(PlayerAnimations.RUN_BACKWARD);
                }
                else if (movement.x < 0)
                    ChangeAnimationState(PlayerAnimations.STRAFE_LEFT);
                else if (movement.x > 0)
                    ChangeAnimationState(PlayerAnimations.STRAFE_RIGHT);
                else if (manager.attackController.GetIsAttacking())
                    ChangeAnimationState(PlayerAnimations.PUNCH_LEFT);
                else
                    ChangeAnimationState(PlayerAnimations.IDLE);
            }
        }
        else // not on ground
        {
            if (!manager.movementController.GetIsFalling() && manager.movementController.GetIsJumping() && velocity.y < 0f)
            {
                ChangeAnimationState(PlayerAnimations.FALLING_LOOP);
            }
        }
    }

    private void ChangeAnimationState(PlayerAnimations newState)
    {
        if (newState == currentState) return;
        animator.CrossFade(newState.GetAnimationName(), k_animationCrossFade);
        currentState = newState;
    }

    public void ChangeAnimationForJump()
    {
        Vector2 movement = manager.movementController.GetMovement();
        if (movement.y != 0)
        {
            Vector3 jumpDirection = new(movement.x, 0, Mathf.Abs(movement.y));
            if (movement.y < 0)
            {
                jumpDirection.x = -jumpDirection.x;
            }
            Vector3 worldMove = transform.TransformDirection(jumpDirection);
            if (worldMove.sqrMagnitude > k_jumpChangeInDirectionThreshold)
            {
                Vector3 lookDirection = new(worldMove.x, 0, worldMove.z);
                manager.modelTransform.rotation = Quaternion.LookRotation(lookDirection);
            }
            ChangeAnimationState(PlayerAnimations.JUMP_WHILE_RUNNING);
        }
        else
        {
            ChangeAnimationState(PlayerAnimations.JUMP_UP);
        }
    }
}
