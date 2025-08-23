using UnityEngine;

public class EnemyKnockback : MonoBehaviour, IEnemyComponent
{
    protected EnemyStats stats;
    protected EnemyControllerBase controllerBase;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    public virtual void ApplyKnockback(Vector3 direction, float force)
    {
    }
}
