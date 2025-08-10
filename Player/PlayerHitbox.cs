using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IHitboxable
{
    Transform player;

    [Header("Values")]
    [SerializeField] private HitboxValues values;
    private Collider Hitbox;
    private HashSet<GameObject> DamagedTargets { get; set; }

    void Awake()
    {
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
                enemyHealth.TakeDamage(values.baseDamage, player.position, values.force);
            }
        }
    }

    public float GetDamageStartTime() { return values.startTime; }
    public float GetDamageEndTime() { return values.endTime; }

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
