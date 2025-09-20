using UnityEngine;

public class Skeleton : EnemyControllerBase
{
    protected override void Awake()
    {
        sizeMultiplier = Random.Range(sizeMultiplierMin, sizeMultiplierMax);

        base.Awake();
    }

    public override void OnSpawned(Vector3 _)
    {
        sizeMultiplier = Random.Range(sizeMultiplierMin, sizeMultiplierMax);

        base.OnSpawned(_);
    }
}
