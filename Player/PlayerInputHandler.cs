using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CharacterController controller;

    private Vector2 moveInput;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private bool HasDetectedGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        manager.movementController.SetMovement(moveInput);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        bool isSprintInput = context.ReadValue<float>() == 1;
        manager.movementController.SetIsSprinting(isSprintInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!manager.movementController.GetIsJumping() && (controller.isGrounded || HasDetectedGround()))
        {
            manager.movementController.SetIsJumpInitiated(true);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!manager.movementController.GetIsJumping() &&
            (controller.isGrounded || HasDetectedGround()) &&
            manager.movementController.GetMovement().Equals(Vector2.zero)
           )
        {
            manager.attackController.SetIsAttacking(true);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            manager.interactHandler.InteractWithClosestInteraction();
        }
    }
}
