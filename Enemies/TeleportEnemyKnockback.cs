using System.Collections;
using UnityEngine;

public class TeleportEnemyKnockback : MonoBehaviour, IEnemyKnockbackable
{
    public EnemyStats stats { get; set; }
    public bool isStunned { get; set; }
    private EnemyControllerBase controllerBase;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        controllerBase.DisableAgent();
        controllerBase.SetDamageState();

        Vector3 randomPos = new(Random.value, 0, Random.value);
        randomPos.Normalize();
        Vector3 roughPos = transform.position +  randomPos * stats.knockbackDistance;
        controllerBase.WarpAgent(roughPos);

        StartCoroutine(KnockbackRoutine(direction, force));
    }

    public IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isStunned = true;
        transform.rotation = Quaternion.LookRotation(-direction);

        yield return new WaitForSeconds(stats.knockbackTime); // TODO: formula for knockback time?

        controllerBase.RestartAgent();
        isStunned = false;
    }
}
