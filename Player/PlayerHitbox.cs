using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IHitboxable
{
    Transform player;

    [Header("Values")]
    [SerializeField] private HitboxValues values;

    private BoxCollider Hitbox;

    [Header("Positioning")]
    [SerializeField] private GameObject originalImpactPoint;
    private GameObject impactPoint;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO punchEnemyEvent;
    [SerializeField] private Vector3EventChannelSO swordHitEnemyEvent;
    private Vector3EventChannelSO hitEnemyEvent;

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
        hitEnemyEvent = punchEnemyEvent;

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
                enemyHealth.TakeDamage(damageValue, player.position, forceValue);
                hitEnemyEvent.OnPositionEventRaised?.Invoke(impactPoint.transform.position);
            }
        }
    }

    public void ApplyWeaponData(WeaponData weaponData, GameObject impactPoint)
    {
        Hitbox.size = weaponData.hitboxSize;
        Hitbox.center = new Vector3(Hitbox.center.x, Hitbox.center.y, weaponData.hitboxSize.z / 2);
        this.impactPoint = impactPoint;

        if (weaponData.IsWeaponOfType(AttackType.SWING))
            hitEnemyEvent = swordHitEnemyEvent;

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
