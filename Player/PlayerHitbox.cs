using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IHitboxable
{
    Transform player;

    public float BaseDamage { get; private set; }
    public float StartTime { get; private set; }
    public float EndTime { get; private set; }
    public float Force { get; private set; }
    public Collider Hitbox { get; private set; }
    public HashSet<GameObject> DamagedTargets { get; set; }

    void Awake()
    {
        BaseDamage = 10f;
        StartTime = 0.6f;
        EndTime = 0.8f;
        Force = 7.5f;
        DamagedTargets = new HashSet<GameObject>();
        Hitbox = GetComponent<Collider>();
        Hitbox.enabled = false;
    }


    void Start()
    {
        player = PlayerManager.Instance;
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Hitbox.enabled) return; // only active during damage window
        if (other.CompareTag("Enemy") && !DamagedTargets.Contains(other.gameObject))
        {
            DamagedTargets.Add(other.gameObject);

            // Damage and death check
            if (other.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(BaseDamage);
            }

            Transform enemyTransform = other.gameObject.transform;
            Vector3 knockbackDirection = (enemyTransform.position - player.position).normalized;
            if (other.gameObject.TryGetComponent(out IEnemyKnockbackable knockback)) {
                knockback.ApplyKnockback(knockbackDirection, Force);
            };
        }
    }

    public void StartAttack()
    {
        DamagedTargets.Clear(); // reset damage tracking for each swing
        Hitbox.enabled = true;
    }

    public void EndAttack()
    {
        Hitbox.enabled = false;
    }
}
