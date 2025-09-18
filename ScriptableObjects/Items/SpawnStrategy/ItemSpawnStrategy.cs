using UnityEngine;

public abstract class ItemSpawnStrategy : ScriptableObject
{
    public float minDistanceFromPlayer;
    public float maxDistanceFromPlayer;

    public abstract bool ShouldSpawn(float deltaTime);
}
