using UnityEngine;

public interface IProjectilible : IPoolable
{
    public abstract void Launch(BowData bowData, Vector3 direction, float speed);
}
