using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IHitboxable
{
    Transform player;

    [Header("Values")]
    [SerializeField] private HitboxValues values;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO getWeaponEvent;

    private BoxCollider Hitbox;

    private HashSet<GameObject> DamagedTargets { get; set; }

    private float damageValue;
    private float forceValue;
    private float damageStartTime;
    private float damageEndTime;

    void Awake()
    {
        DamagedTargets = new HashSet<GameObject>();
        Hitbox = GetComponent<BoxCollider>();
        Hitbox.size = values.size;
        Hitbox.enabled = false;

        damageValue = values.baseDamage;
        forceValue = values.force;
        damageStartTime = values.startTime;
        damageEndTime = values.endTime;
    }

    void Start()
    {
        player = PlayerManager.Instance;

        getWeaponEvent.OnWeaponEvent += ApplyWeapon;
    }

    void OnDestroy()
    {
        getWeaponEvent.OnWeaponEvent -= ApplyWeapon;
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
                enemyHealth.TakeDamage(damageValue, player.position, forceValue);
            }
        }
    }

    public void ApplyWeapon(WeaponData weaponData)
    {
        Debug.Log($"using {weaponData.name} weapon values");
        Hitbox.size = weaponData.hitboxSize;
        Hitbox.center = new Vector3(Hitbox.center.x, Hitbox.center.y, weaponData.hitboxSize.z / 2);

        damageValue += weaponData.damage;
        forceValue += weaponData.knockbackForce;
        damageStartTime = values.weaponStartTime;
        damageEndTime = values.weaponEndTime;
    }

    public float GetDamageStartTime() { return damageStartTime; }
    public float GetDamageEndTime() { return damageEndTime; }

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
