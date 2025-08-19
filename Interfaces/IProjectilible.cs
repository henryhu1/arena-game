using UnityEngine;

public interface IProjectilible : IPoolable
{
    public abstract void Launch(float damagePoints, Vector3 direction, float speed);
}
