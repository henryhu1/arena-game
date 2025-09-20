using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private List<EnemySpawnData> spawnConfigs;

    [SerializeField] private FloatVariable timeElapsed;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO spawnEnemyEvent;
    [SerializeField] private EnemyEventChannelSO defeatedEventChannel;
    [SerializeField] private Vector3EventChannelSO despawnEnemyEvent;
    [SerializeField] private VoidEventChannelSO allWaveEnemiesDefeatedEventChannel;
    [SerializeField] private IntEventChannelSO roundStartedEventChannel;

    private Dictionary<EnemySpawnData, EnemySpawnStrategy> activeStrategies = new();
    public float TimeElapsed => timeElapsed.GetValue();

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

    private void OnEnable()
    {
        defeatedEventChannel.OnEnemyEvent += HandleEnemyDefeated;
        roundStartedEventChannel.OnEventRaised += SpawnWave;
    }

    private void OnDisable()
    {
        defeatedEventChannel.OnEnemyEvent -= HandleEnemyDefeated;
        roundStartedEventChannel.OnEventRaised -= SpawnWave;
    }

    private void Update()
    {
        foreach (var strategy in activeStrategies.Values)
        {
            if (strategy is not TimeBasedSpawnStrategySO) continue;
            strategy.UpdateStrategy(Time.deltaTime);
        }
    }

    public void SpawnWave(int round)
    {
        foreach (var data in spawnConfigs)
        {
            Debug.Log($"is spawning wave: {data.spawnStrategy is WaveBasedSpawnStrategy}");
            if (data.spawnStrategy is not WaveBasedSpawnStrategy) continue;

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

        Vector3 pos = PlayerManager.Instance.GetRandomPositionAroundPlayer(data.minDistanceFromPlayer, data.maxDistanceFromPlayer);
        GameObject enemy = ObjectPoolManager.Instance.Spawn(data.enemyPrefab, pos, Quaternion.identity);
        spawnEnemyEvent.OnPositionEventRaised(pos);

        // var tracker = enemy.AddComponent<EnemyLifetimeTracker>();
        // tracker.spawnData = data;
        // tracker.spawner = this;

        data.currentAlive++;

        if (data.spawnStrategy is WaveBasedSpawnStrategy)
            TotalWaveEnemiesAlive++;

    }

    private void HandleEnemyDefeated(EnemyControllerBase enemy)
    {
        if (enemy.GetSpawnStrategy() is WaveBasedSpawnStrategy)
        {
            TotalWaveEnemiesAlive--;
            if (TotalWaveEnemiesAlive == 0)
            {
                allWaveEnemiesDefeatedEventChannel.RaiseEvent();
            }
        }
    }

    public void DespawnEnemy(GameObject obj, EnemySpawnData data)
    {
        ObjectPoolManager.Instance.Despawn(obj, data.enemyPrefab);
        despawnEnemyEvent.OnPositionEventRaised(obj.transform.position);
    }
}
