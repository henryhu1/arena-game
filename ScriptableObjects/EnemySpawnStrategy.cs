using UnityEngine;

public abstract class EnemySpawnStrategy : ScriptableObject
{
    public abstract void Initialize(EnemySpawner spawner, EnemySpawnData data);
    public abstract void UpdateStrategy(float deltaTime);
}
