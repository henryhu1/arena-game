using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [SerializeField] private List<ItemSpawnStrategy> spawnConfigs;

    private Dictionary<GameObject, ItemSpawnStrategy> activeStrategies = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        foreach (var strategy in spawnConfigs)
        {
            activeStrategies[strategy.itemPrefab] = strategy;
        }
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
}
