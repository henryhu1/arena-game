using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IHitboxable
{
    Transform player;

    [Header("Values")]
    [SerializeField] private HitboxValues values;

    private BoxCollider Hitbox;

    [Header("Positioning")]
    [SerializeField] private Transform originalImpactPoint;
    private Transform impactPoint;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO onWeaponUse;

    private HashSet<GameObject> DamagedTargets { get; set; }

    private Weapon weaponInUse;
    private float damageValue = 0;
    private float forceValue = 0;
    private Vector3 originalCenter;

    void Awake()
    {
        DamagedTargets = new HashSet<GameObject>();
        Hitbox = GetComponent<BoxCollider>();
        Hitbox.size = values.size;
        Hitbox.enabled = false;

        impactPoint = originalImpactPoint;
        originalCenter = Hitbox.center;

        ResetHitboxValues();
    }

    public void ResetHitboxValues()
    {
        damageValue = values.baseDamage;
        forceValue = values.force;
        Hitbox.size = values.size;
        Hitbox.center = originalCenter;
        weaponInUse = null;
    }

    void Start()
    {
        player = PlayerManager.Instance.transform;
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!Hitbox.enabled || GameManager.Instance.IsGameOver()) return; // only active during damage window
        if (other.CompareTag("Enemy") && !DamagedTargets.Contains(other.gameObject))
        {
            DamagedTargets.Add(other.gameObject);

            // Damage and death check
            if (other.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(other.ClosestPoint(transform.position), damageValue, player.position, forceValue);
                if (weaponInUse != null)
                {
                    onWeaponUse.RaiseEvent(weaponInUse);
                }
            }
        }
    }

    public void ApplyWeaponData(Weapon weapon)
    {
        weaponInUse = weapon;
        WeaponData weaponData = weapon.GetWeaponData();
        Transform impactPoint = weapon.GetImpactPoint();
        Hitbox.size = weaponData.hitboxSize;
        Hitbox.center = new Vector3(Hitbox.center.x, Hitbox.center.y, weaponData.hitboxSize.z / 2);
        this.impactPoint = impactPoint;

        damageValue += weaponData.damage;
        forceValue += weaponData.knockbackForce;
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
