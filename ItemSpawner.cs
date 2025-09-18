using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [SerializeField] private List<CollectableItem> spawnableItems;

    private Dictionary<CollectableItem, ItemSpawnStrategy> activeStrategies = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        foreach (var data in spawnableItems)
        {
            var strategy = Instantiate(data.spawnStrategy);
            activeStrategies[data] = strategy;
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
                GameObject item = ObjectPoolManager.Instance.Spawn(itemSpawn.Key.gameObject, pos, Quaternion.identity);
            }
        }
    }
}
