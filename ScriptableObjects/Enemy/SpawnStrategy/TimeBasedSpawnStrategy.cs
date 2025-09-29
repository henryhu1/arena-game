using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Spawn Strategy/Time")]
public class TimeBasedSpawnStrategySO : EnemySpawnStrategy
{
    public AnimationCurve spawnRateOverTime;
    public AnimationCurve spawnCountOverTime;

    private EnemySpawner spawner;
    private EnemySpawnData data;
    private float spawnCooldown;

    public override void Initialize(EnemySpawner spawner, EnemySpawnData data)
    {
        this.spawner = spawner;
        this.data = data;
    }

    public override void UpdateStrategy(float deltaTime, int currentRound)
    {
        float time = spawner.TimeElapsed;
        float rate = spawnRateOverTime.Evaluate(time);
        spawnCooldown -= deltaTime;

        if (spawnCooldown <= 0f && data.currentAlive < data.GetMaxAliveForRound(currentRound))
        {
            int count = Mathf.RoundToInt(spawnCountOverTime.Evaluate(time));
            for (int i = 0; i < count; i++)
                spawner.SpawnEnemy(data);

            spawnCooldown = 1f / Mathf.Max(rate, 0.01f);
        }
    }
}
