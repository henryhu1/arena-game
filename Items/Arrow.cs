using UnityEngine;

public class Arrow : MonoBehaviour, IProjectilible
{
    [SerializeField] private float defaultSpeed = 20f;
    [SerializeField] private float defaultForce = 5f;
    protected float damagePoints = 0;

    private BowData shotFromBow;

    [Header("Arrow Parts")]
    public Transform arrowTip;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO arrowHitEvent;

    private Rigidbody rb;
    private CollectableItem collectableItem;

    private bool hasHit = false;

    protected void Awake()
    {
        collectableItem = GetComponent<CollectableItem>();
        rb = GetComponent<Rigidbody>();

        // Shift center of mass toward tip
        Vector3 localTip = transform.InverseTransformPoint(arrowTip.position);
        rb.centerOfMass = localTip;

        // Ensure correct collision settings
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnDisable()
    {
        shotFromBow.ReturnArrowToPool(gameObject);
    }

    public virtual void OnSpawned(Vector3 position)
    {
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero; // reset
        rb.angularVelocity = Vector3.zero;
        transform.position = position;
    }

    public virtual void OnDespawned()
    {
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (!hasHit && rb.linearVelocity.sqrMagnitude > 0.01f)
            transform.forward = rb.linearVelocity.normalized;
    }

    // TODO: move to enemy controller base to implement its own collision detection
    //   avoid projectiles for teleporting enemies
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(arrowTip.position, damagePoints, transform.position, defaultForce);
            }
        }

        if (gameObject.layer == LayerMask.NameToLayer("DamageDealing"))
        {
            StickArrow(other);
        }

        arrowHitEvent.RaiseEvent(arrowTip.position);
    }

    public void Launch(BowData bowData, Vector3 direction, float speed)
    {
        hasHit = false;

        MakeUninteractable();

        float launchSpeed = speed > 0 ? speed : defaultSpeed;

        direction.Normalize();

        // orient arrow
        arrowTip.forward = MainCamera.Instance.transform.forward;

        // shoot arrow
        rb.linearVelocity = direction * launchSpeed;

        shotFromBow = bowData;
        damagePoints = bowData.damagePoints;
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
