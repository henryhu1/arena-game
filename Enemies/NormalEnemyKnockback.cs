using System.Collections;
using UnityEngine;

public class NormalEnemyKnockback : MonoBehaviour, IEnemyKnockbackable
{
    public EnemyStats stats { get; set; }
    private EnemyControllerBase controllerBase;

    private Coroutine knockedBackStunBuffer;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        controllerBase.DisableAgent();
        controllerBase.SetDamageState();

        if (knockedBackStunBuffer != null)
        {
            StopCoroutine(knockedBackStunBuffer);
        }
        knockedBackStunBuffer = StartCoroutine(KnockbackRoutine(direction, force));
    }

    public IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        transform.rotation = Quaternion.LookRotation(-direction);

        yield return new WaitForSeconds(stats.knockbackTime); // TODO: formula for knockback time?

        controllerBase.RestartAgent();
        knockedBackStunBuffer = null;
    }
}
