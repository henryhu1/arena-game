using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [SerializeField] private List<ItemSpawnStrategy> spawnConfigs;

    private Dictionary<GameObject, ItemSpawnStrategy> activeStrategies = new();
    private Dictionary<EnemyDropSpawnStrategy, float> itemDropRates = new();

    // TODO: may want each enemy to have their own base drop rate;
    [SerializeField] private float baseEnemyDropRate = 0.5f;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO onEnemyDespawn;
    [SerializeField] private Vector3EventChannelSO itemSpawnEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        float totalSpawnChances = 0;
        foreach (var strategy in spawnConfigs)
        {
            activeStrategies[strategy.itemPrefab] = strategy;
            if (strategy is EnemyDropSpawnStrategy enemyDropStrategy)
            {
                totalSpawnChances += enemyDropStrategy.spawnChance;
            }
        }

        foreach (var strategy in spawnConfigs)
        {
            if (strategy is EnemyDropSpawnStrategy enemyDropStrategy)
            {
                float dropPercentage = enemyDropStrategy.spawnChance / totalSpawnChances;
                itemDropRates[enemyDropStrategy] = dropPercentage;
            }
        }
    }

    private void OnEnable()
    {
        onEnemyDespawn.OnPositionEventRaised += AttemptItemDrop;
    }

    private void OnDisable()
    {
        onEnemyDespawn.OnPositionEventRaised -= AttemptItemDrop;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        foreach (var itemSpawn in activeStrategies)
        {
            if (itemSpawn.Value.ShouldSpawn(Time.deltaTime))
            {
                Vector3 pos = PlayerManager.Instance.GetRandomPositionAroundPlayer(itemSpawn.Value.minDistanceFromPlayer, itemSpawn.Value.maxDistanceFromPlayer);
                GameObject item = ObjectPoolManager.Instance.Spawn(itemSpawn.Value.itemPrefab, pos, Quaternion.identity);
            }
        }
    }

    public void DespawnItem(GameObject obj, GameObject prefab)
    {
        // TODO: item to despawn and prefab args are always together
        //   lookup? skip the call to item spawner?
        ObjectPoolManager.Instance.Despawn(obj, prefab);
    }

    private void AttemptItemDrop(Vector3 pos)
    {
        if (GameRoundManager.Instance.CurrentRound <= 0) return;

        float baseAttempt = Random.value;
        if (baseAttempt > baseEnemyDropRate) return;

        float itemDropAttempt = Random.value;
        float accumulation = 0;
        foreach (var itemSpawn in itemDropRates)
        {
            if (itemDropAttempt < accumulation + itemSpawn.Value)
            {
                Vector3 spawnPos = NavMeshUtils.GetPositionOnNavMesh(pos);
                GameObject item = ObjectPoolManager.Instance.Spawn(itemSpawn.Key.itemPrefab, spawnPos, Quaternion.identity);
                itemSpawnEvent.RaiseEvent(item.transform.position);
                return;
            }
            accumulation += itemSpawn.Value;
        }
    }
}
