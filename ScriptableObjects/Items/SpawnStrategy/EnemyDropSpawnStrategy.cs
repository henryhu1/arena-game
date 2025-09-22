using UnityEngine;

[CreateAssetMenu(menuName = "Items/Spawn Strategy/Enemy Drop")]
public class EnemyDropSpawnStrategy : ItemSpawnStrategy
{
    public float spawnChance;
    public override bool ShouldSpawn(float deltaTime)
    {
        return false;
    }
}
