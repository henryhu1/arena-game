using System.Collections.Generic;
using UnityEngine;

public interface IHitboxable
{
    float BaseDamage { get; }
    float StartTime { get; }
    float EndTime { get; }
    float Force { get; }
    Collider Hitbox { get; }
    HashSet<GameObject> DamagedTargets { get; set; }

    void StartAttack();
    void EndAttack();
}
