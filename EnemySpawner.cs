using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private List<EnemySpawnData> spawnConfigs;

    [Header("Data")]
    [SerializeField] private FloatVariable timeElapsed;
    [SerializeField] private IntVariable enemiesDefeated;
    [SerializeField] private IntVariable CurrentRound;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO spawnEnemyEvent;
    [SerializeField] private EnemyEventChannelSO defeatedEventChannel;
    [SerializeField] private Vector3EventChannelSO despawnEnemyEvent;
    [SerializeField] private VoidEventChannelSO allWaveEnemiesDefeatedEventChannel;
    [SerializeField] private IntEventChannelSO roundStartedEventChannel;
    [SerializeField] private VoidEventChannelSO onGameRestart;

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

        enemiesDefeated.ResetValue();
    }

    private void OnEnable()
    {
        defeatedEventChannel.OnEnemyEvent += HandleEnemyDefeated;
        roundStartedEventChannel.OnEventRaised += SpawnWave;
        onGameRestart.OnEventRaised += Reset;
    }

    private void OnDisable()
    {
        defeatedEventChannel.OnEnemyEvent -= HandleEnemyDefeated;
        roundStartedEventChannel.OnEventRaised -= SpawnWave;
        onGameRestart.OnEventRaised -= Reset;
    }

    private void Update()
    {
        foreach (var strategy in activeStrategies.Values)
        {
            if (strategy is not TimeBasedSpawnStrategySO) continue;
            strategy.UpdateStrategy(Time.deltaTime, CurrentRound.GetValue());
        }
    }

    public void SpawnWave(int round)
    {
        if (round == 0) return;

        foreach (var data in spawnConfigs)
        {
#if UNITY_EDITOR
            Debug.Log($"is spawning wave: {data.spawnStrategy is WaveBasedSpawnStrategy}");
#endif
            if (data.spawnStrategy is not WaveBasedSpawnStrategy) continue;
            if (data.startAfterRound >= round) continue;

            int maxThisRound = data.GetMaxAliveForRound(round - data.startAfterRound);
            int toSpawn = maxThisRound - data.currentAlive;

#if UNITY_EDITOR
            Debug.Log($"spawn count: {maxThisRound} - {data.currentAlive} = {toSpawn}");
#endif
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnEnemy(data);
            }
        }
    }

    public void SpawnEnemy(EnemySpawnData data)
    {
        if (data.currentAlive >= data.GetMaxAliveForRound(CurrentRound.GetValue()))
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
        enemiesDefeated.AddToValue(1);
        if (enemy.GetSpawnStrategy() is WaveBasedSpawnStrategy)
        {
            TotalWaveEnemiesAlive--;
            if (TotalWaveEnemiesAlive == 0)
            {
                allWaveEnemiesDefeatedEventChannel.RaiseEvent();
            }
        }
    }

    private void Reset()
    {
        TotalWaveEnemiesAlive = 0;
        foreach (var data in spawnConfigs)
        {
            data.ResetCurrentAlive();
        }
    }

    public void DespawnEnemy(GameObject obj, EnemySpawnData data)
    {
        ObjectPoolManager.Instance.Despawn(obj, data.enemyPrefab);
        despawnEnemyEvent.OnPositionEventRaised(obj.transform.position);
    }
}
