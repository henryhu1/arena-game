using System.Collections;
using UnityEngine;

public class NormalEnemyKnockback : MonoBehaviour, IEnemyKnockbackable
{
    public EnemyStats stats { get; set; }
    private EnemyControllerBase controllerBase;

    private float knockbackTime;
    private float knockbackDistance;

    private Coroutine buffer;

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

        if (buffer != null)
        {
            StopCoroutine(buffer);
        }
        buffer = StartCoroutine(KnockbackRoutine(direction, force));
    }

    public IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        transform.rotation = Quaternion.LookRotation(-direction);

        yield return new WaitForSeconds(knockbackTime); // TODO: formula for knockback time?

        controllerBase.RestartAgent();
        buffer = null;
    }
}
