using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    public int maxHealth = 100;
    public float moveSpeed = 3.5f;
    public int damage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackStart = 0.1f;
    public float attackEnd = 0.4f;
    public float knockbackTime = 1f;
    public float knockbackDistance = 3f;
    public AnimationClip attackClip;
    public float AttackClipLength => attackClip != null ? attackClip.length : 0f;
    public AnimationClip damageClip;
    public float DamageClipLength => attackClip != null ? attackClip.length : 0f;
    public AnimationClip deathClip;
    public float DeathClipLength => deathClip != null ? deathClip.length : 0f;
}
