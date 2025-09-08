using UnityEngine;

public class TeleportEnemyKnockback : EnemyKnockback
{
    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO enemyTeleportEvent;

    public override void ApplyKnockback(Vector3 direction, float force)
    {
        base.ApplyKnockback(direction, force);
        enemyTeleportEvent.RaiseEvent(transform.position);

        Vector3 randomPos = new(Random.value, 0, Random.value);
        randomPos.Normalize();
        Vector3 roughPos = transform.position + randomPos * stats.KnockbackDistance();
        Vector3 teleportedToPos = controllerBase.WarpAgent(roughPos);

        Debug.Log($"teleport to {teleportedToPos}, but at {transform.position}");
        enemyTeleportEvent.RaiseEvent(teleportedToPos);
    }

    public override bool IsKnockbackableByProjectile() { return false; }
}
