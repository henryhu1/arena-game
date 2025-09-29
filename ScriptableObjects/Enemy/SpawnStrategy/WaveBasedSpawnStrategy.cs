using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Spawn Strategy/Wave")]
public class WaveBasedSpawnStrategy : EnemySpawnStrategy
{
    private EnemySpawner spawner;
    private EnemySpawnData data;

    public override void Initialize(EnemySpawner spawner, EnemySpawnData data)
    {
        this.spawner = spawner;
        this.data = data;
    }

    public override void UpdateStrategy(float deltaTime, int currentRound)
    {
        int maxAlive = data.GetMaxAliveForRound(currentRound);
        int toSpawn = maxAlive - data.currentAlive;

        for (int i = 0; i < toSpawn; i++)
            spawner.SpawnEnemy(data);
    }
}
