using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: figure out camera collision with terrain and obstacles
public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    private CameraFocus objectToOrbit;

    [Header("Control")]
    [SerializeField] private PlayerInput cameraControl;
    
    // TODO: use SO data, as setting configs
    public bool isInverted = true;

    [Header("Values")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float radius = 10;
    [SerializeField] private float followSpeed = 10;
    [SerializeField] private float verticalFollowSlack = 10;
    [SerializeField] private float verticalRotationLimit = 40f;
    private float currentPitch;
    private Vector2 rotation;

    [Header("Music")]
    [SerializeField] private AudioSource inGameAudioSource;
    [SerializeField] private AudioSource outGameAudioSource;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;
    [SerializeField] private BoolEventChannelSO onGamePauseToggle;

    [Header("Collision")]
    public LayerMask collisionLayers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        var focusableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<CameraFocus>();
        objectToOrbit = focusableObjects.FirstOrDefault();
        collisionLayers = LayerMask.GetMask("Ground");
    }

    private void OnEnable()
    {
        onGamePauseToggle.OnEventRaised += GamePausedToggleHandler;
        onGameOver.OnEventRaised += GameOverEventHandler;
        onGameRestart.OnEventRaised += GameRestartEventHandler;
    }

    private void OnDisable()
    {
        onGamePauseToggle.OnEventRaised -= GamePausedToggleHandler;
        onGameOver.OnEventRaised -= GameOverEventHandler;
        onGameRestart.OnEventRaised -= GameRestartEventHandler;
    }

    void Update()
    {
        Vector3 followingObjectPosition = objectToOrbit.GetFocusPointPosition();
        transform.RotateAround(followingObjectPosition, Vector3.up, rotation.x * sensitivity);

        int vertical = isInverted ? 1 : -1;

        float pitchChange = vertical * rotation.y * sensitivity;
        float newPitch = Mathf.Clamp(currentPitch + pitchChange, -verticalRotationLimit, verticalRotationLimit);
        float clampedPitchChange = newPitch - currentPitch;
        transform.RotateAround(followingObjectPosition, transform.right, clampedPitchChange);
        currentPitch = newPitch;

        Vector3 cameraPosition = followingObjectPosition - (transform.forward * radius);
        if (!objectToOrbit.DoesWantFocus())
        {
            if (Mathf.Abs(followingObjectPosition.y - transform.position.y) < verticalFollowSlack)
            {
                cameraPosition.y = transform.position.y;
            }
        }
        transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        rotation = context.ReadValue<Vector2>();
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
    }

    private void GamePausedToggleHandler(bool isPaused)
    {
        if (isPaused)
        {
            cameraControl.enabled = false;
            inGameAudioSource.Pause();
            outGameAudioSource.Play();
        }
        else
        {
            cameraControl.enabled = true;
            inGameAudioSource.Play();
            outGameAudioSource.Pause();
        }
    }

    private void GameOverEventHandler()
    {
        cameraControl.enabled = false;
        inGameAudioSource.Pause();
        outGameAudioSource.Play();
    }

    private void GameRestartEventHandler()
    {
        cameraControl.enabled = true;
        inGameAudioSource.Play();
        outGameAudioSource.Pause();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Camera collided with: " + other.name);
    //    if (IsCollisionValid(other))
    //    {
    //        Debug.Log("collision valid enter");
    //        // Adjust the camera position or take action (e.g., stop movement)
    //        //Vector3 directionAway = (transform.position - other.ClosestPoint(transform.position)).normalized;
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("Camera collided with: " + other.name);
    //    if (IsCollisionValid(other))
    //    {
    //        Debug.Log("collision valid stay");
    //        // Continuously adjust the camera while colliding
    //    }
    //}

    //private bool IsCollisionValid(Collider other)
    //{
    //    // Only detect objects in the specified collision layers
    //    return ((1 << other.gameObject.layer) & collisionLayers) != 0;
    //}


    //// Handle the collision by slightly moving the camera away
    //private void HandleCollision(Collider other)
    //{
    //    // Calculate the direction to move the camera away from the collision
    //    Vector3 directionAway = (transform.position - other.ClosestPoint(transform.position)).normalized;

    //    // Adjust the camera's position to avoid clipping
    //    transform.position += directionAway * 0.2f;  // Adjust the multiplier as needed
    //}
}
