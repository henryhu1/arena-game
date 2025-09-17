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

    private HashSet<GameObject> DamagedTargets { get; set; }

    private float damageValue = 0;
    private float forceValue = 0;

    void Awake()
    {
        DamagedTargets = new HashSet<GameObject>();
        Hitbox = GetComponent<BoxCollider>();
        Hitbox.size = values.size;
        Hitbox.enabled = false;

        impactPoint = originalImpactPoint;

        ResetHitboxValues();
    }

    private void ResetHitboxValues()
    {
        damageValue = values.baseDamage;
        forceValue = values.force;
    }

    private void AddToHitboxValues(WeaponData weaponData)
    {
        damageValue += weaponData.damage;
        forceValue += weaponData.knockbackForce;
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
            }
        }
    }

    public void ApplyWeaponData(Weapon weapon)
    {
        WeaponData weaponData = weapon.GetWeaponData();
        Transform impactPoint = weapon.GetImpactPoint();
        Hitbox.size = weaponData.hitboxSize;
        Hitbox.center = new Vector3(Hitbox.center.x, Hitbox.center.y, weaponData.hitboxSize.z / 2);
        this.impactPoint = impactPoint;

        AddToHitboxValues(weaponData);
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
