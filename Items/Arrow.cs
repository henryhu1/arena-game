using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField] private float defaultSpeed = 20f;
    [SerializeField] private float defaultForce = 5f;
    protected float damagePoints = 0;

    [Header("Arrow Parts")]
    public Transform arrowTip; // Assign actual arrow tip mesh position

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
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnDisable()
    {
        ObjectPoolManager.Instance.Despawn(gameObject);
    }

    void FixedUpdate()
    {
        if (!hasHit && rb.linearVelocity.sqrMagnitude > 0.01f)
            transform.forward = rb.linearVelocity.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damagePoints, transform.position, defaultForce);
            }
        }

        if (gameObject.layer == LayerMask.NameToLayer("DamageDealing"))
        {
            StickArrow(other);
        }
    }

    public override void Launch(float damagePoints, Vector3 direction, float speed)
    {
        MakeUninteractable();

        float launchSpeed = speed > 0 ? speed : defaultSpeed;

        direction.Normalize();

        // orient arrow
        arrowTip.forward = MainCamera.Instance.transform.forward;

        // shoot arrow
        rb.linearVelocity = direction * launchSpeed;

        this.damagePoints = damagePoints;
    }

    private void StickArrow(Collider hitCollider)
    {
        hasHit = true;
        damagePoints = 0;

        // Stop physics
        rb.isKinematic = true;

        // Parent to hit object so it moves with it
        transform.parent = hitCollider.transform;

        // Snap position so the tip stays exactly where it hit
        // if (Physics.Raycast(arrowTip.position, transform.forward, out RaycastHit hit, 0.5f))
        // {
        //     Vector3 offset = transform.position - arrowTip.position;
        //     transform.position = hit.point + offset;
        // }

        MakeInteractable();
    }

    private void MakeInteractable()
    {
        collectableItem.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Interactables");
    }

    private void MakeUninteractable()
    {
        collectableItem.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("DamageDealing");
    }
}
