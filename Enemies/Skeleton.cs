using UnityEngine;

public class Skeleton : EnemyControllerBase
{
    protected override void Awake()
    {
        enemyStats.sizeMultiplier = Random.Range(enemyStats.SizeMultiplierMin(), enemyStats.SizeMultiplierMax());

        base.Awake();
    }
}
