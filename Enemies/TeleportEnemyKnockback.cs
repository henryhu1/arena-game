using System.Collections;
using UnityEngine;

public class TeleportEnemyKnockback : MonoBehaviour, IEnemyKnockbackable
{
    public EnemyStats stats { get; set; }
    private EnemyControllerBase controllerBase;

    private float knockbackTime;
    private float knockbackDistance;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Start()
    {
        knockbackTime = stats.knockbackTime;
        knockbackDistance = stats.knockbackDistance;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        controllerBase.DisableAgent();
        controllerBase.SetDamageState();

        Vector3 roughPos = transform.position + direction * knockbackDistance;
        roughPos.y = 100f;
        float maxDistance = 200f;

        controllerBase.WarpAgent(roughPos, maxDistance);

        StartCoroutine(KnockbackRoutine(direction, force));
    }

    public IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        transform.rotation = Quaternion.LookRotation(-direction);

        yield return new WaitForSeconds(knockbackTime); // TODO: formula for knockback time?

        controllerBase.RestartAgent();
    }
}
