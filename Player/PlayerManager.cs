using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerInteractHandler))]
[RequireComponent(typeof(PlayerAttackController))]
public class PlayerManager : MonoBehaviour
{
    public static Transform Instance { get; private set; }

    [Header("Player Components")]
    public PlayerHealth health { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public PlayerMovementController movementController { get; private set; }
    public PlayerAnimationController animationController { get; private set; }
    public PlayerInteractHandler interactHandler { get; private set; }
    public PlayerAttackController attackController { get; private set; }

    [Header("Camera Focus Point")]
    public CameraFocus focusPoint;

    [Header("Player Model")]
    public Transform modelTransform;

    [SerializeField] private CharacterController controller;

    private void Awake()
    {
        Instance = transform;
        focusPoint.SetWantsFocus();

        health = GetComponent<PlayerHealth>();
        inputHandler = GetComponent<PlayerInputHandler>();
        movementController = GetComponent<PlayerMovementController>();
        animationController = GetComponent<PlayerAnimationController>();
        interactHandler = GetComponent<PlayerInteractHandler>();
        attackController = GetComponent<PlayerAttackController>();

        InitializeAll();
    }

    void InitializeAll()
    {
        health.Initialize(this);
        inputHandler.Initialize(this);
        movementController.Initialize(this);
        animationController.Initialize(this);
        interactHandler.Initialize(this);
        attackController.Initialize(this);
    }

    private void Update()
    {
        if (controller.isGrounded)
        {
            if (movementController.GetVelocity().y <= 0f)
            {
                if (movementController.GetIsFalling())
                {
                    focusPoint.SetWantsFocus();
                }
            }
        }
    }
}
