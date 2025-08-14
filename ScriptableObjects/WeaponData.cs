using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Items/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Weapon";

    [Header("Stats")]
    public float damage = 10f;
    public float knockbackForce = 5f;
    public float attackCooldown = 0.5f; // TODO: utilize attack cooldown

    [Header("Hitbox Settings")]
    public Vector3 hitboxSize = new(0.7f, 1.7f, 1f);
    public Vector3 hitboxOffset = Vector3.zero;
}
