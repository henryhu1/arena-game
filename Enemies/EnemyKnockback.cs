using System.Collections;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour, IEnemyComponent
{
    protected EnemyStats stats;
    protected EnemyControllerBase controllerBase;
    protected Coroutine knockedBackStunBuffer;
    protected bool isStunned;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    public bool GetIsStunned() { return isStunned; }

    public virtual void ApplyKnockback(Vector3 direction, float force)
    {
        if (knockedBackStunBuffer != null)
        {
            StopCoroutine(knockedBackStunBuffer);
        }
        knockedBackStunBuffer = StartCoroutine(KnockbackRoutine(direction, force));
    }

    protected IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isStunned = true;
        transform.rotation = Quaternion.LookRotation(-direction);

        yield return new WaitForSeconds(stats.knockbackTime); // TODO: formula for knockback time?

        controllerBase.RestartAgent();
        knockedBackStunBuffer = null;
        isStunned = false;
    }
}
