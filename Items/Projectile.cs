using UnityEngine;

public abstract class Projectile : PoolIdentity, IPoolable
{
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public abstract void Launch(Vector3 direction, float speed);

    // Called automatically by pool on spawn
    public virtual void OnSpawned(Vector3 position)
    {
        rb.linearVelocity = Vector3.zero; // reset
        rb.angularVelocity = Vector3.zero;
        transform.position = position;
    }

    // Called automatically by pool on despawn
    public virtual void OnDespawned()
    {
        if (rb.isKinematic) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
