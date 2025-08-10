using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private List<EnemySpawnData> spawnConfigs;

    [SerializeField] private FloatReference timeElapsed;
    [SerializeField] private float spawnCheckInterval = 2f;

    [Header("Events")]
    [SerializeField] private EnemyDeathEventChannelSO deathEventChannel;
    [SerializeField] private VoidEventChannelSO allWaveEnemiesDefeatedEventChannel;
    [SerializeField] private IntEventChannelSO roundStartedEventChannel;

    private float timer;

    private Transform player;

    private Dictionary<EnemySpawnData, EnemySpawnStrategy> activeStrategies = new();
    public float TimeElapsed => timeElapsed?.Value ?? 0f;

    public int TotalWaveEnemiesAlive { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        foreach (var data in spawnConfigs)
        {
            data.ResetCurrentAlive();
            var strategy = Instantiate(data.spawnStrategy);
            strategy.Initialize(this, data);
            activeStrategies[data] = strategy;
        }
    }

    private void Start()
    {
        player = PlayerManager.Instance;

        deathEventChannel.OnEnemyDied += HandleEnemyDeath;
        roundStartedEventChannel.OnEventRaised += SpawnWave;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // if (timer >= spawnCheckInterval)
        // {
        //     timer = 0f;
        //     SpawnEnemies();
        // }

        // foreach (var strategy in activeStrategies.Values)
        // {
        //     strategy.UpdateStrategy(Time.deltaTime);
        // }
    }

    private void OnDestroy()
    {
        deathEventChannel.OnEnemyDied -= HandleEnemyDeath;
        roundStartedEventChannel.OnEventRaised -= SpawnWave;
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

    private void HandleEnemyDeath(EnemyControllerBase enemy)
    {
        enemy.spawnData.currentAlive--;
        if (enemy.spawnData.spawnStrategy is WaveBasedSpawnStrategy)
        {
            TotalWaveEnemiesAlive--;
            if (TotalWaveEnemiesAlive == 0)
            {
                allWaveEnemiesDefeatedEventChannel.RaiseEvent();
            }
        }
    }

    private Vector3 GetRandomSpawnPosition(float minDist, float maxDist)
    {
        Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(minDist, maxDist);
        Vector3 offset = new(offset2D.x, 0f, offset2D.y);
        Vector3 roughPos = player.position + offset;
        Vector3 spawnPos = EnemyPositionUtils.GetPositionOnNavMesh(roughPos);
        if (spawnPos != Vector3.negativeInfinity)
        {
            return spawnPos;
        }
        return roughPos;
    }

    public void DespawnEnemy(GameObject obj, EnemySpawnData data)
    {
        ObjectPoolManager.Instance.Despawn(obj, data.enemyPrefab);
    }
}
