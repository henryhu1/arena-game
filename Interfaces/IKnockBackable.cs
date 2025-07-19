using System.Collections;
using UnityEngine;

public interface IKnockBackable
{
    Rigidbody Rb { get; }
    float KnockbackTime { get; }
    float KnockbackDistance { get; }

    void ApplyKnockback(Vector3 direction, float force);

    IEnumerator KnockbackRoutine(Vector3 direction, float force);
}
