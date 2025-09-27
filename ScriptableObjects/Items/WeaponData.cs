using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Items/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public enum HeldByHand
    {
        RIGHT,
        LEFT,
    }

    public string weaponKey = "Weapon";

    public AttackType attackType;
    public AudioEffectSO contactAudio;

    [Header("Stats")]
    public float damage = 10f;
    public float knockbackForce = 5f;
    public float attackCooldown = 0.5f; // TODO: utilize attack cooldown

    [Header("Hitbox Settings")]
    public Vector3 hitboxSize = new(0.7f, 1.7f, 1f);
    public Vector3 hitboxOffset = Vector3.zero;

    [Header("Model")]
    public HeldByHand heldByHand = HeldByHand.RIGHT;
    public Vector3 eulerRotation;

    public bool IsWeaponOfType(AttackType type) => attackType == type;
}
