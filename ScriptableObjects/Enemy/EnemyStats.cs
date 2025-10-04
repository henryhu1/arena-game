using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 3.5f;

    [Header("Gameplay")]
    public float pointValue = 1;
    public float timeRegained = 10;

    [Header("Attack Stats")]
    public int damage = 10;
    public float attackRange = 2f;
    public float attackStart = 0.1f;
    public float attackEnd = 0.4f;
    public float attackCooldown = 1;

    [Header("Drop Rate")]
    [Range(0, 1)] public float dropRate = 0.5f;

    [Header("Knockback Stats")]
    public float knockbackTime = 1f;
    public float knockbackDistance = 3f;

    [Header("Animations")]
    public List<EnemyAnimationName> animationMappings = new();
    private Dictionary<EnemyAnimation, string> lookup;

    public AnimationClip attackClip;
    public AnimationClip damageClip;
    public AnimationClip deathClip;

    [Header("Audio Effects")]
    public EnemyAudioProfileSO audioProfile;

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
}
