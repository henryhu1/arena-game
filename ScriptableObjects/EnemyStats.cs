using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Stats")]
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
}
