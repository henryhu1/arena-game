using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Attack Stats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackStart = 0.1f;
    [SerializeField] private float attackEnd = 0.4f;
    [SerializeField] private float attackCooldown = 1;

    [Header("Knockback Stats")]
    [SerializeField] private float knockbackTime = 1f;
    [SerializeField] private float knockbackDistance = 3f;

    [Header("Animations")]
    [SerializeField] private List<EnemyAnimationName> animationMappings = new();
    private Dictionary<EnemyAnimation, string> lookup;

    [SerializeField] private AnimationClip attackClip;
    [SerializeField] private AnimationClip damageClip;
    [SerializeField] private AnimationClip deathClip;

    [Header("Audio Effects")]
    [SerializeField] private EnemyAudioProfileSO audioProfile;

    [Header("Stat Multiplier")]
    [SerializeField] private float sizeMultiplierMin = 1;
    [SerializeField] private float sizeMultiplierMax = 1;
    [SerializeField] public float sizeMultiplier = 1;

    public string GetAnimationName(EnemyAnimation key)
    {
        if (lookup == null)
        {
            lookup = new Dictionary<EnemyAnimation, string>();
            foreach (var pair in animationMappings)
                lookup[pair.key] = pair.value;
        }
        return lookup.TryGetValue(key, out var val) ? val : null;
    }

    public float MaxHealth() => maxHealth * sizeMultiplier;
    public float MoveSpeed() => moveSpeed / sizeMultiplier;
    public float Damage() => damage * sizeMultiplier;
    public float AttackRange() => attackRange * sizeMultiplier;
    public float AttackStart() => attackStart;
    public float AttackEnd() => attackEnd;
    public float AttackCooldown() => attackCooldown;
    public float KnockbackTime() => knockbackTime * sizeMultiplier;
    public float KnockbackDistance() => knockbackDistance * sizeMultiplier;
    public float SizeMultiplierMin() => sizeMultiplierMin;
    public float SizeMultiplierMax() => sizeMultiplierMax;
    public EnemyAudioProfileSO GetAudioProfile() => audioProfile;
}
