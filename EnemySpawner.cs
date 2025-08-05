using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnData> spawnConfigs;

    [SerializeField] private FloatReference timeElapsed;
    [SerializeField] private float spawnCheckInterval = 2f;
    private float timer;

    private Transform player;

    private Dictionary<EnemySpawnData, EnemySpawnStrategy> activeStrategies = new();
    public float TimeElapsed => timeElapsed?.Value ?? 0f;

    public int TotalWaveEnemiesAlive { get; private set; } = 0;

    private void Start()
    {
        player = PlayerManager.Instance;

        foreach (var data in spawnConfigs)
        {
            data.currentAlive = 0;
            var strategy = Instantiate(data.spawnStrategy);
            strategy.Initialize(this, data);
            activeStrategies[data] = strategy;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // if (timer >= spawnCheckInterval)
        // {
        //     timer = 0f;
        //     SpawnEnemies();
        // }

        foreach (var strategy in activeStrategies.Values)
        {
            strategy.UpdateStrategy(Time.deltaTime);
        }
    }

    // private void SpawnEnemies()
    // {
    //     foreach (var spawnData in spawnConfigs)
    //     {
    //         if (spawnData.currentAlive >= spawnData.maxAlive) continue;

    //         float time = timeElapsed.Value;

    //         float rate = spawnData.spawnRateOverTime.Evaluate(time);
    //         if (Random.value > rate) continue; // Skip this type this cycle

    //         int count = Mathf.RoundToInt(spawnData.spawnCountOverTime.Evaluate(time));
    //         int availableSlots = spawnData.maxAlive - spawnData.currentAlive;
    //         int finalCount = Mathf.Min(spawnCount, availableSlots);

    //         for (int i = 0; i < finalCount; i++)
    //         {
    //             Vector3 spawnPos = GetRandomSpawnPosition(spawnData.minDistanceFromPlayer, spawnData.maxDistanceFromPlayer);
    //             Instantiate(spawnData.enemyPrefab, spawnPos, Quaternion.identity);
    //             spawnData.currentAlive++;
    //         }
    //     }
    // }

    public void SpawnWave(int round)
    {
        foreach (var data in spawnConfigs)
        {
            Debug.Log($"is spawning wave: {data.spawnStrategy is WaveBasedSpawnStrategy}");
            if (data.spawnStrategy is not WaveBasedSpawnStrategy) return;

            int maxThisRound = data.GetMaxAliveForRound(round);
            int toSpawn = maxThisRound - data.currentAlive;

            Debug.Log($"spawn count: {maxThisRound} - {data.currentAlive} = {toSpawn}");
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnEnemy(data);
            }
        }
    }

    public void SpawnEnemy(EnemySpawnData data)
    {
        if (data.currentAlive >= data.GetMaxAliveForRound(GameRoundManager.Instance.CurrentRound))
            return;

        Vector3 pos = GetRandomSpawnPosition(data.minDistanceFromPlayer, data.maxDistanceFromPlayer);
        GameObject enemy = ObjectPoolManager.Instance.Spawn(data.enemyPrefab, pos, Quaternion.identity);

        // var tracker = enemy.AddComponent<EnemyLifetimeTracker>();
        // tracker.spawnData = data;
        // tracker.spawner = this;

        data.currentAlive++;

        if (data.spawnStrategy is WaveBasedSpawnStrategy)
            TotalWaveEnemiesAlive++;

    }

    private Vector3 GetRandomSpawnPosition(float minDist, float maxDist)
    {
        Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(minDist, maxDist);
        Vector3 offset = new(offset2D.x, 0f, offset2D.y);
        return player.position + offset;
    }

    public void DespawnEnemy(EnemySpawnData data)
    {
        ObjectPoolManager.Instance.Despawn(gameObject, data.enemyPrefab);
    }
}
