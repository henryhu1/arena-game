using System.Collections;
using UnityEngine;

public interface IEnemyKnockbackable : IEnemyComponent
{
  EnemyStats stats { get; set; }
  bool isStunned { get; set; }

  void ApplyKnockback(Vector3 direction, float force);
  IEnumerator KnockbackRoutine(Vector3 direction, float force);
}
