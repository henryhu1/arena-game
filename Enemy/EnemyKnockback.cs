using UnityEngine;

public class EnemyKnockback : MonoBehaviour, IEnemyComponent
{
    protected EnemyControllerBase controllerBase;

    public void Initialize(EnemyControllerBase controllerBase)
    {
        this.controllerBase = controllerBase;
    }

    public virtual void ApplyKnockback(Vector3 direction, float force)
    {
    }

    public virtual bool IsKnockbackableByProjectile() { return true; }
}
