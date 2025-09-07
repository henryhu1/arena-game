using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerInteractHandler))]
[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerInventoryHandler))]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Player Components")]
    public PlayerHealth health { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public PlayerMovementController movementController { get; private set; }
    public PlayerAnimationController animationController { get; private set; }
    public PlayerInteractHandler interactHandler { get; private set; }
    public PlayerAttackController attackController { get; private set; }
    public PlayerInventoryHandler inventoryHandler { get; private set; }

    [Header("Camera Focus Point")]
    public CameraFocus focusPoint;

    [Header("Player Model")]
    public Transform modelTransform;
    public GameObject rightGripPoint;
    public GameObject leftGripPoint;
    public GameObject heldArrow;
    public GameObject projectileSpawnPoint;

    [SerializeField] private CharacterController controller;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        focusPoint.SetWantsFocus();

        health = GetComponent<PlayerHealth>();
        inputHandler = GetComponent<PlayerInputHandler>();
        movementController = GetComponent<PlayerMovementController>();
        animationController = GetComponent<PlayerAnimationController>();
        interactHandler = GetComponent<PlayerInteractHandler>();
        attackController = GetComponent<PlayerAttackController>();
        inventoryHandler = GetComponent<PlayerInventoryHandler>();

        InitializeAll();
        heldArrow.SetActive(false);
    }

    void InitializeAll()
    {
        health.Initialize(this);
        inputHandler.Initialize(this);
        movementController.Initialize(this);
        animationController.Initialize(this);
        interactHandler.Initialize(this);
        attackController.Initialize(this);
        inventoryHandler.Initialize(this);
    }

    private void Update()
    {
        if (IsControllerGrounded())
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

    public bool IsControllerGrounded()
    {
        return controller.isGrounded;
    }

    public void MovePlayer(Vector3 velocity)
    {
        controller.Move(Time.deltaTime * velocity);
        transform.position = controller.transform.position;
    }

    public AudioEffectSO GetAttackAudio()
    {
        return attackController.GetAttackAudio();
    }
}
