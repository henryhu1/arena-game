using UnityEngine;

// TODO: make prefab
//   combine player hitbox prefab as one hitbox prefab
public class EnemyHitbox : MonoBehaviour, IHitboxable
{
    private BoxCollider Hitbox;

    private float damagePoints;
    private bool hasDealtDamage;

    private void Awake()
    {
        Hitbox = GetComponent<BoxCollider>();
        Hitbox.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage) return;

        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(other.ClosestPoint(transform.position), damagePoints);
            hasDealtDamage = true;
        }
    }

    public void Setup(float damagePoints, float attackRange)
    {
        this.damagePoints = damagePoints;
        Hitbox.size = Vector3.one * attackRange;
        Hitbox.center = new Vector3(0, 0.5f, 0.5f) * attackRange;
    }

    public void StartAttack()
    {
        hasDealtDamage = false;
        Hitbox.enabled = true;
    }

    public void EndAttack()
    {
        Hitbox.enabled = false;
    }
}
