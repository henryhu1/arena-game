using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CharacterController controller;

    [SerializeField] private VoidEventChannelSO OnTakeDamage;
    [SerializeField] private VoidEventChannelSO OnFreeFromDamage;
    [SerializeField] private VoidEventChannelSO OnDeath;

    private Vector2 movement;
    private bool isSprinting;
    private bool isJumpInitiated;
    private bool isJumping;
    // private bool jumpAirTime;
    // private bool jumpHeightReached;
    private bool isFalling;
    private bool isPreventedFromMoving;

    [Header("Movement Settings")]
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float speed = 10;
    [SerializeField] private float strafeSpeed = 6;
    [SerializeField] private float backwardSpeed = 5;
    [SerializeField] private float sprintModifier = 2;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float jumpDelay = 0.3f;
    [SerializeField] private Vector3 velocity = Vector3.zero;
    const float k_downwardForce = -10f;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void SetMovement(Vector2 input)
    {
        this.movement = input;
    }

    public Vector2 GetMovement()
    {
        return this.movement;
    }

    public bool IsMoving()
    {
        return this.movement != Vector2.zero;
    }

    private void PlayerHealth_OnTakeDamage() { isPreventedFromMoving = true; }

    private void PlayerHealth_OnFreeFromDamage() { isPreventedFromMoving = false; }
    private void PlayerHealth_OnDeath() { isPreventedFromMoving = true; }

    public Vector3 GetVelocity() { return this.velocity; }

    public void SetIsSprinting(bool isSprintInput)
    {
        this.isSprinting = isSprintInput;
    }

    public bool GetIsSprinting() {  return this.isSprinting; }

    public void SetIsJumpInitiated(bool isJumpInput)
    {
        this.isJumpInitiated = isJumpInput;
    }

    public bool GetIsJumpInitiated()
    {
        return this.isJumpInitiated;
    }

    public bool GetIsJumping()
    {
        return this.isJumping;
    }

    public bool GetIsFalling()
    {
        return this.isFalling;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        isPreventedFromMoving = false;
    }

    private void Start()
    {
        OnTakeDamage.OnEventRaised += PlayerHealth_OnTakeDamage;
        OnFreeFromDamage.OnEventRaised += PlayerHealth_OnFreeFromDamage;
        OnDeath.OnEventRaised += PlayerHealth_OnDeath;
    }

    private void OnDestroy()
    {
        OnTakeDamage.OnEventRaised -= PlayerHealth_OnTakeDamage;
        OnFreeFromDamage.OnEventRaised -= PlayerHealth_OnFreeFromDamage;
        OnDeath.OnEventRaised += PlayerHealth_OnDeath;
    }

    private void FixedUpdate()
    {
        if (isPreventedFromMoving)
        {
            if (velocity != Vector3.zero) velocity = Vector3.zero;
            return;
        }

        Vector3 cameraDirection = GetCameraLookDirection();
        Vector2 flattenedForward = new(cameraDirection.x, cameraDirection.z);
        Vector2 sidewaysToCamera = -Vector2.Perpendicular(flattenedForward);

        float playerSpeed = (isSprinting ? sprintModifier : 1) * speed;
        float forwardBackwardSpeed = (movement.y >= 0 ? playerSpeed : backwardSpeed) * movement.y;
        float sidewaySpeed = (movement.y == 0 ? strafeSpeed : playerSpeed) * movement.x;

        Vector3 sidewaysDirection = new(sidewaysToCamera.x, 0, sidewaysToCamera.y);
        Vector3 groundVelocity = CalculateMovement(sidewaySpeed, sidewaysDirection) + CalculateMovement(forwardBackwardSpeed, cameraDirection);

        velocity.x = groundVelocity.x;
        velocity.z = groundVelocity.z;
    }

    private void Update()
    {
        if (isPreventedFromMoving)
        {
            return;
        }

        // move player controller and player
        controller.Move(Time.deltaTime * velocity);
        transform.position = controller.transform.position;

        Vector3 cameraDirection = GetCameraLookDirection();
        // make player face the same direction as the camera
        transform.forward = cameraDirection;

        if (controller.isGrounded)
        {
            if (isJumpInitiated)
            {
                isJumpInitiated = false;
                StartCoroutine(PrepareJump());
            }

            if (velocity.y <= 0f)
            {
                velocity.y = k_downwardForce;
                if (isJumping && !isFalling)
                {

                }
                else if (isFalling)
                {
                    isFalling = false;
                    isJumping = false;
                    manager.focusPoint.SetWantsFocus();
                    manager.modelTransform.rotation = Quaternion.LookRotation(GetCameraLookDirection());
                }
            }
        }
        else // not on ground
        {
            velocity.y += gravity * Time.deltaTime;

            if (!isFalling && isJumping && velocity.y < 0f)
            {
                isFalling = true;
            }
        }
    }

    private IEnumerator PrepareJump()
    {
        manager.focusPoint.StopWantingFocus();

        isJumping = true;

        manager.animationController.ChangeAnimationForJump();

        yield return null;

        velocity.y = jumpForce;
    }

    private Vector3 CalculateMovement(float speed, Vector3 direction) => speed * direction;

    private Vector3 GetCameraLookDirection()
    {
        Vector3 cameraDirection = MainCamera.Instance.transform.forward;
        cameraDirection.y = 0;
        cameraDirection.Normalize();
        return cameraDirection;
    }

}
