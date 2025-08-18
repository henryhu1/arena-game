using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField] private float defaultSpeed = 20f;

    [Header("Arrow Parts")]
    public Transform arrowTip; // Assign actual arrow tip mesh position
    public Collider shaftCollider; // Main flight collider
    public Collider tipCollider;   // Trigger collider at tip

    private CollectableItem collectableItem;

    private bool hasHit = false;

    protected override void Awake()
    {
        base.Awake();

        collectableItem = GetComponent<CollectableItem>();
        rb = GetComponent<Rigidbody>();

        // Shift center of mass toward tip
        Vector3 localTip = transform.InverseTransformPoint(arrowTip.position);
        rb.centerOfMass = localTip;

        // Ensure correct collision settings
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void FixedUpdate()
    {
        if (!hasHit && rb.linearVelocity.sqrMagnitude > 0.01f)
            transform.forward = rb.linearVelocity.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other == shaftCollider) return; // Ignore self

        if (gameObject.layer == LayerMask.NameToLayer("DamageDealing"))
        {
            StickArrow(other);
        }
    }

    public override void Launch(Vector3 direction, float speed)
    {
        collectableItem.enabled = false;

        float launchSpeed = speed > 0 ? speed : defaultSpeed;

        direction.Normalize();

        // orient arrow
        arrowTip.forward = MainCamera.Instance.transform.forward;

        // shoot arrow
        rb.linearVelocity = direction * launchSpeed;
    }

    private void StickArrow(Collider hitCollider)
    {
        hasHit = true;

        // Stop physics
        rb.isKinematic = true;

        // Disable shaft collisions to prevent knock-off
        shaftCollider.enabled = false;
        tipCollider.enabled = false;

        // Parent to hit object so it moves with it
        transform.parent = hitCollider.transform;

        // Snap position so the tip stays exactly where it hit
        // if (Physics.Raycast(arrowTip.position, transform.forward, out RaycastHit hit, 0.5f))
        // {
        //     Vector3 offset = transform.position - arrowTip.position;
        //     transform.position = hit.point + offset;
        // }
    }
}
