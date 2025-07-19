using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    private const float k_verticalRotationLimit = 40;

    [SerializeField] private ICameraFocusable objectToOrbit;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float radius = 10;
    [SerializeField] private float followSpeed = 10;
    [SerializeField] private float verticalFollowSlack = 10;

    public LayerMask collisionLayers;

    public bool isInverted = false;
    private float currentPitch;
    private Vector2 rotation;

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
        var focusableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICameraFocusable>();
        objectToOrbit = focusableObjects.FirstOrDefault();
        collisionLayers = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        Vector3 followingObjectPosition = objectToOrbit.FocusPoint.position;
        transform.RotateAround(followingObjectPosition, Vector3.up, rotation.x * sensitivity);

        int vertical = isInverted ? -1 : 1;

        float pitchChange = vertical * rotation.y * sensitivity;
        float newPitch = Mathf.Clamp(currentPitch + pitchChange, -k_verticalRotationLimit, k_verticalRotationLimit);
        float clampedPitchChange = newPitch - currentPitch;
        transform.RotateAround(followingObjectPosition, transform.right, clampedPitchChange);
        currentPitch = newPitch;

        Vector3 cameraPosition = followingObjectPosition - (transform.forward * radius);
        if (!objectToOrbit.WantsFocus)
        {
            if (Mathf.Abs(followingObjectPosition.y - transform.position.y) < verticalFollowSlack) {
                cameraPosition.y = transform.position.y;
            }
        }
        transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        rotation = context.ReadValue<Vector2>();
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
