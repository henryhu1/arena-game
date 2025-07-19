using UnityEngine;

public class PlayerManager : MonoBehaviour, ICameraFocusable
{
    public static Transform Instance { get; private set; }
    public Transform FocusPoint { get; set; }
    public bool WantsFocus { get; set; }

    public PlayerInputHandler inputHandler { get; private set; }
    public PlayerMovementController movementController { get; private set; }
    public PlayerAnimationController animationController { get; private set; }
    public PlayerEquipables equipables { get; private set; }
    public PlayerAttackController attackController { get; private set; }

    [Header("Camera Focus Point")]
    public GameObject FocusPointGameObject;

    [Header("Player Model Components")]
    public Transform modelTransform;

    [SerializeField] private CharacterController controller;

    private void Awake()
    {
        Instance = transform;
        FocusPoint = FocusPointGameObject.transform;
        WantsFocus = true;

        inputHandler = GetComponent<PlayerInputHandler>();
        movementController = GetComponent<PlayerMovementController>();
        animationController = GetComponent<PlayerAnimationController>();
        equipables = GetComponent<PlayerEquipables>();
        attackController = GetComponent<PlayerAttackController>();

        InitializeAll();
    }

    void InitializeAll()
    {
        inputHandler.Initialize(this);
        movementController.Initialize(this);
        animationController.Initialize(this);
        equipables.Initialize(this);
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
                    WantsFocus = true;
                }
            }
        }
    }
}
