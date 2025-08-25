using UnityEngine;

public class TeleportEnemyKnockback : EnemyKnockback
{
    public override void ApplyKnockback(Vector3 direction, float force)
    {
        base.ApplyKnockback(direction, force);

        Vector3 randomPos = new(Random.value, 0, Random.value);
        randomPos.Normalize();
        Vector3 roughPos = transform.position +  randomPos * stats.KnockbackDistance();
        controllerBase.WarpAgent(roughPos);
    }
}
