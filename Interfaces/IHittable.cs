using UnityEngine;

public interface IHittable
{
    public bool TakeHit(Vector3 contactPos, float damagePoints, Vector3 fromDirection, float force);
}