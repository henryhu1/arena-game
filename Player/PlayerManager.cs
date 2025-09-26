using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerInteractHandler))]
[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerInventoryHandler))]
[RequireComponent(typeof(PlayerAudioController))]
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
    public PlayerAudioController audioController { get; private set; }
    [SerializeField] private PlayerInput playerInputComponent;
    [SerializeField] private CharacterController controller;

    [Header("Camera Focus Point")]
    public CameraFocus focusPoint;

    [Header("Player Model")]
    public Transform modelTransform;
    public GameObject projectileSpawnPoint;

    // TODO: instead of each individual player component subscribing to an event,
    //   this manager will hold all the relevant events, then the player components
    //   override an abstract / implement a virtual function called by the manager
    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;

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
        audioController = GetComponent<PlayerAudioController>();

        InitializeAll();
    }

    private void Start()
    {
        EnablePlayerInput();
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
        audioController.Initialize(this);
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

    private void OnEnable()
    {
        onGameOver.OnEventRaised += DisablePlayerInput;
        onGameRestart.OnEventRaised += EnablePlayerInput;
    }

    private void OnDisable()
    {
        onGameOver.OnEventRaised -= DisablePlayerInput;
        onGameRestart.OnEventRaised -= EnablePlayerInput;
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

    public Vector3 GetRandomPositionAroundPlayer(float minDist, float maxDist)
    {
        Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(minDist, maxDist);
        Vector3 offset = new(offset2D.x, 0f, offset2D.y);
        Vector3 roughPos = transform.position + offset;
        Vector3 onNavMeshPos = NavMeshUtils.GetPositionOnNavMesh(roughPos);
        if (onNavMeshPos != Vector3.negativeInfinity)
        {
            return onNavMeshPos;
        }
        return roughPos;
    }

    public AudioEffectSO GetAttackAudio()
    {
        return attackController.GetAttackAudio();
    }

    private void DisablePlayerInput()
    {
        playerInputComponent.enabled = false;
    }

    private void EnablePlayerInput()
    {
        playerInputComponent.enabled = true;
    }
}
